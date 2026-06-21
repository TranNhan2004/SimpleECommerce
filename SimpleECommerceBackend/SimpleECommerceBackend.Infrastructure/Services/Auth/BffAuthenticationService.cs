using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Auth;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Infrastructure.Options.Authentication;

namespace SimpleECommerceBackend.Infrastructure.Services.Auth;

public sealed class BffAuthenticationService : IBffAuthenticationService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheService _cacheService;
    private readonly AuthOptions _authOptions;
    private readonly KeycloakBffOptions _keycloakOptions;
    private readonly Serilog.ILogger _logger;

    public BffAuthenticationService(
        IHttpClientFactory httpClientFactory,
        ICacheService cacheService,
        IOptions<AuthOptions> authOptions,
        IOptions<KeycloakBffOptions> keycloakOptions,
        Serilog.ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _cacheService = cacheService;
        _authOptions = authOptions.Value;
        _keycloakOptions = keycloakOptions.Value;
        _logger = logger;
    }

    public async Task<BeginLoginResult> BeginLoginAsync(string? returnUrl, CancellationToken cancellationToken = default)
    {
        var normalizedReturnPath = NormalizeReturnPath(returnUrl);
        var state = CreateToken();
        var codeVerifier = CreateToken();
        var codeChallenge = CreateCodeChallenge(codeVerifier);

        var stateRecord = new OAuthStateRecord
        {
            State = state,
            CodeVerifier = codeVerifier,
            ReturnPath = normalizedReturnPath,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await _cacheService.SetAsync(
            AuthCacheKeys.GetOAuthStateKey(state),
            stateRecord,
            TimeSpan.FromMinutes(_authOptions.OAuthStateExpireMinutes),
            cancellationToken
        );

        return new BeginLoginResult
        {
            AuthorizationUrl = BuildAuthorizationUrl(state, codeChallenge)
        };
    }

    public async Task<CompleteLoginResult> CompleteLoginAsync(
        CompleteLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(command.Error))
        {
            throw new UnauthorizedException(
                AuthErrorCodes.OAuthProviderError,
                $"Identity provider returned error '{command.Error}': {command.ErrorDescription}"
            );
        }

        if (string.IsNullOrWhiteSpace(command.State))
        {
            throw new ValidationException(AuthErrorCodes.OAuthStateMissing, "OAuth callback state is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Code))
        {
            throw new ValidationException(AuthErrorCodes.OAuthCodeMissing, "OAuth callback code is required.");
        }

        var stateKey = AuthCacheKeys.GetOAuthStateKey(command.State);
        var stateRecord = await _cacheService.GetAsync<OAuthStateRecord>(stateKey, cancellationToken);
        if (stateRecord is null)
        {
            throw new UnauthorizedException(AuthErrorCodes.OAuthStateInvalid, "OAuth state is missing or expired.");
        }

        await _cacheService.RemoveAsync(stateKey, cancellationToken);

        var tokenResponse = await ExchangeCodeForTokensAsync(command.Code, stateRecord.CodeVerifier, cancellationToken);
        var userInfo = await FetchUserInfoAsync(tokenResponse.AccessToken, cancellationToken);

        if (!Guid.TryParse(userInfo.Subject, out _))
        {
            throw new UnauthorizedException(
                CurrentUserErrorCodes.SubjectIdInvalid,
                $"Current keycloak subject id claim 'sub' is invalid: {userInfo.Subject}."
            );
        }

        var sessionId = CreateToken();
        var csrfToken = CreateToken();
        var now = DateTimeOffset.UtcNow;
        var accessTokenExpiresAtUtc = now.AddSeconds(Math.Max(tokenResponse.ExpiresIn, 1));
        var refreshTokenExpiresAtUtc = tokenResponse.RefreshExpiresIn > 0
            ? now.AddSeconds(tokenResponse.RefreshExpiresIn)
            : now.AddMinutes(_authOptions.SessionExpireMinutes);
        var sessionTtl = CalculateSessionTtl(now, refreshTokenExpiresAtUtc);
        var sessionExpiresAtUtc = now.Add(sessionTtl);

        var session = new AuthSessionSnapshot
        {
            SessionId = sessionId,
            Subject = userInfo.Subject,
            Email = userInfo.Email ?? string.Empty,
            PreferredUsername = userInfo.PreferredUsername ?? userInfo.Email ?? userInfo.Subject,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            IdToken = tokenResponse.IdToken,
            CsrfToken = csrfToken,
            SessionExpiresAtUtc = sessionExpiresAtUtc,
            AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            CreatedAtUtc = now,
            LastRefreshedAtUtc = now
        };

        await _cacheService.SetAsync(
            AuthCacheKeys.GetSessionKey(sessionId),
            session,
            sessionTtl,
            cancellationToken
        );

        return new CompleteLoginResult
        {
            SessionId = session.SessionId,
            CsrfToken = session.CsrfToken,
            FrontendRedirectUrl = BuildFrontendUrl(stateRecord.ReturnPath),
            SessionExpiresAtUtc = session.SessionExpiresAtUtc,
            AccessTokenExpiresAtUtc = session.AccessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc = session.RefreshTokenExpiresAtUtc
        };
    }

    public async Task<LogoutResult> LogoutAsync(
        string? sessionId,
        string? csrfCookieToken,
        string? csrfHeaderToken,
        CancellationToken cancellationToken = default)
    {
        var frontendRedirectUrl = BuildFrontendUrl(_authOptions.PostLogoutRedirectPath);

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return new LogoutResult
            {
                FrontendRedirectUrl = frontendRedirectUrl
            };
        }

        var sessionKey = AuthCacheKeys.GetSessionKey(sessionId);
        var session = await _cacheService.GetAsync<AuthSessionSnapshot>(sessionKey, cancellationToken);
        if (session is not null)
        {
            ValidateCsrf(session, csrfCookieToken, csrfHeaderToken);
            await _cacheService.RemoveAsync(sessionKey, cancellationToken);
        }

        return new LogoutResult
        {
            FrontendRedirectUrl = frontendRedirectUrl,
            IdentityProviderLogoutUrl = BuildIdentityProviderLogoutUrl(session?.IdToken)
        };
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync(
        string sessionId,
        string? csrfCookieToken,
        string? csrfHeaderToken,
        CancellationToken cancellationToken = default)
    {
        var sessionKey = AuthCacheKeys.GetSessionKey(sessionId);
        var session = await _cacheService.GetAsync<AuthSessionSnapshot>(sessionKey, cancellationToken);
        if (session is null)
        {
            throw new UnauthorizedException(AuthErrorCodes.SessionNotFound, "Authentication session was not found.");
        }

        ValidateCsrf(session, csrfCookieToken, csrfHeaderToken);

        if (string.IsNullOrWhiteSpace(session.RefreshToken))
        {
            throw new UnauthorizedException(AuthErrorCodes.RefreshTokenMissing, "Refresh token is missing.");
        }

        var tokenResponse = await RefreshTokensAsync(session.RefreshToken, cancellationToken);
        var userInfo = await FetchUserInfoAsync(tokenResponse.AccessToken, cancellationToken);

        if (!string.Equals(session.Subject, userInfo.Subject, StringComparison.Ordinal))
        {
            throw new UnauthorizedException(
                AuthErrorCodes.TokenExchangeFailed,
                "Refreshed token subject does not match the existing session."
            );
        }

        var now = DateTimeOffset.UtcNow;
        var accessTokenExpiresAtUtc = now.AddSeconds(Math.Max(tokenResponse.ExpiresIn, 1));
        var refreshTokenExpiresAtUtc = tokenResponse.RefreshExpiresIn > 0
            ? now.AddSeconds(tokenResponse.RefreshExpiresIn)
            : session.RefreshTokenExpiresAtUtc;
        var sessionTtl = CalculateSessionTtl(now, refreshTokenExpiresAtUtc);
        var updatedSession = new AuthSessionSnapshot
        {
            SessionId = session.SessionId,
            Subject = session.Subject,
            Email = userInfo.Email ?? session.Email,
            PreferredUsername = userInfo.PreferredUsername ?? session.PreferredUsername,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = string.IsNullOrWhiteSpace(tokenResponse.RefreshToken) ? session.RefreshToken : tokenResponse.RefreshToken,
            IdToken = string.IsNullOrWhiteSpace(tokenResponse.IdToken) ? session.IdToken : tokenResponse.IdToken,
            CsrfToken = session.CsrfToken,
            SessionExpiresAtUtc = now.Add(sessionTtl),
            AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            CreatedAtUtc = session.CreatedAtUtc,
            LastRefreshedAtUtc = now
        };

        await _cacheService.SetAsync(sessionKey, updatedSession, sessionTtl, cancellationToken);

        return new RefreshTokenResult
        {
            SessionExpiresAtUtc = updatedSession.SessionExpiresAtUtc,
            AccessTokenExpiresAtUtc = updatedSession.AccessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc = updatedSession.RefreshTokenExpiresAtUtc
        };
    }

    public Task<AuthSessionSnapshot?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return _cacheService.GetAsync<AuthSessionSnapshot>(AuthCacheKeys.GetSessionKey(sessionId), cancellationToken);
    }

    private async Task<KeycloakTokenResponse> ExchangeCodeForTokensAsync(
        string code,
        string codeVerifier,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _keycloakOptions.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string?>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = _keycloakOptions.ClientId,
                ["client_secret"] = _keycloakOptions.ClientSecret,
                ["code"] = code,
                ["redirect_uri"] = _keycloakOptions.RedirectUri,
                ["code_verifier"] = codeVerifier
            }!)
        };

        return await SendTokenRequestAsync(request, cancellationToken);
    }

    private async Task<KeycloakTokenResponse> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _keycloakOptions.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string?>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _keycloakOptions.ClientId,
                ["client_secret"] = _keycloakOptions.ClientSecret,
                ["refresh_token"] = refreshToken
            }!)
        };

        return await SendTokenRequestAsync(request, cancellationToken);
    }

    private async Task<KeycloakTokenResponse> SendTokenRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using var client = CreateHttpClient();
        using var response = await client.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Warning(
                "Keycloak token request failed with status code {StatusCode}. Payload: {Payload}",
                (int)response.StatusCode,
                payload
            );

            throw new UnauthorizedException(AuthErrorCodes.TokenExchangeFailed, "Failed to exchange token with Keycloak.");
        }

        var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(payload, JsonOptions);
        if (tokenResponse is null
            || string.IsNullOrWhiteSpace(tokenResponse.AccessToken)
            || string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
        {
            throw new UnauthorizedException(AuthErrorCodes.TokenExchangeFailed, "Keycloak token response is invalid.");
        }

        return tokenResponse;
    }

    private async Task<KeycloakUserInfoResponse> FetchUserInfoAsync(string accessToken, CancellationToken cancellationToken)
    {
        using var client = CreateHttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, _keycloakOptions.UserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Warning(
                "Keycloak userinfo request failed with status code {StatusCode}. Payload: {Payload}",
                (int)response.StatusCode,
                payload
            );

            throw new UnauthorizedException(AuthErrorCodes.UserInfoFailed, "Failed to retrieve user information from Keycloak.");
        }

        var userInfo = JsonSerializer.Deserialize<KeycloakUserInfoResponse>(payload, JsonOptions);
        if (userInfo is null || string.IsNullOrWhiteSpace(userInfo.Subject))
        {
            throw new UnauthorizedException(AuthErrorCodes.UserInfoFailed, "Keycloak user information is invalid.");
        }

        return userInfo;
    }

    private HttpClient CreateHttpClient()
    {
        var client = _httpClientFactory.CreateClient(nameof(BffAuthenticationService));
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    private string BuildAuthorizationUrl(string state, string codeChallenge)
    {
        var query = new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["client_id"] = _keycloakOptions.ClientId,
            ["redirect_uri"] = _keycloakOptions.RedirectUri,
            ["scope"] = string.Join(' ', _keycloakOptions.Scopes),
            ["state"] = state,
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = "S256"
        };

        return $"{_keycloakOptions.AuthorizationEndpoint}?{BuildQueryString(query)}";
    }

    private string? BuildIdentityProviderLogoutUrl(string? idTokenHint)
    {
        if (string.IsNullOrWhiteSpace(idTokenHint))
            return null;

        var query = new Dictionary<string, string?>
        {
            ["id_token_hint"] = idTokenHint,
            ["post_logout_redirect_uri"] = BuildFrontendUrl(_authOptions.PostLogoutRedirectPath),
            ["client_id"] = _keycloakOptions.ClientId
        };

        return $"{_keycloakOptions.EndSessionEndpoint}?{BuildQueryString(query)}";
    }

    private void ValidateCsrf(AuthSessionSnapshot session, string? csrfCookieToken, string? csrfHeaderToken)
    {
        if (string.IsNullOrWhiteSpace(csrfCookieToken) || string.IsNullOrWhiteSpace(csrfHeaderToken))
        {
            throw new ForbiddenException(AuthErrorCodes.CsrfTokenMissing, "CSRF token is required.");
        }

        if (!string.Equals(session.CsrfToken, csrfCookieToken, StringComparison.Ordinal)
            || !string.Equals(session.CsrfToken, csrfHeaderToken, StringComparison.Ordinal))
        {
            throw new ForbiddenException(AuthErrorCodes.CsrfTokenInvalid, "CSRF token is invalid.");
        }
    }

    private TimeSpan CalculateSessionTtl(DateTimeOffset now, DateTimeOffset refreshTokenExpiresAtUtc)
    {
        var configuredTtl = TimeSpan.FromMinutes(_authOptions.SessionExpireMinutes);
        var refreshTtl = refreshTokenExpiresAtUtc - now;

        if (refreshTtl <= TimeSpan.Zero)
        {
            throw new UnauthorizedException(AuthErrorCodes.RefreshTokenMissing, "Refresh token has already expired.");
        }

        return refreshTtl < configuredTtl ? refreshTtl : configuredTtl;
    }

    private string NormalizeReturnPath(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return NormalizeRelativePath(_authOptions.PostLoginRedirectPath);

        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var absoluteUri))
        {
            var frontendBaseUri = new Uri(_authOptions.FrontendBaseUrl);
            if (!Uri.Compare(frontendBaseUri, absoluteUri, UriComponents.SchemeAndServer, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase).Equals(0))
            {
                throw new ValidationException(AuthErrorCodes.InvalidReturnUrl, "Return URL must belong to the configured frontend.");
            }

            return NormalizeRelativePath($"{absoluteUri.AbsolutePath}{absoluteUri.Query}");
        }

        return NormalizeRelativePath(returnUrl);
    }

    private string BuildFrontendUrl(string path)
    {
        var baseUri = new Uri(_authOptions.FrontendBaseUrl.TrimEnd('/') + "/");
        var normalizedPath = NormalizeRelativePath(path);
        return new Uri(baseUri, normalizedPath.TrimStart('/')).ToString();
    }

    private static string NormalizeRelativePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "/";

        if (!path.StartsWith('/') || path.StartsWith("//", StringComparison.Ordinal))
        {
            throw new ValidationException(AuthErrorCodes.InvalidReturnUrl, "Return URL must be an application-relative path.");
        }

        return path;
    }

    private static string CreateToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string CreateCodeChallenge(string codeVerifier)
    {
        var bytes = Encoding.UTF8.GetBytes(codeVerifier);
        var hash = SHA256.HashData(bytes);
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string BuildQueryString(IReadOnlyDictionary<string, string?> parameters)
    {
        return string.Join(
            "&",
            parameters
                .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
                .Select(parameter => $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value!)}")
        );
    }

    private sealed class OAuthStateRecord
    {
        public string State { get; init; } = null!;
        public string CodeVerifier { get; init; } = null!;
        public string ReturnPath { get; init; } = null!;
        public DateTimeOffset CreatedAtUtc { get; init; }
    }

    private sealed class KeycloakTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = null!;
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; init; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = null!;
        [JsonPropertyName("id_token")]
        public string? IdToken { get; init; }
    }

    private sealed class KeycloakUserInfoResponse
    {
        [JsonPropertyName("sub")]
        public string Subject { get; init; } = null!;
        [JsonPropertyName("email")]
        public string? Email { get; init; }
        [JsonPropertyName("preferred_username")]
        public string? PreferredUsername { get; init; }
    }
}
