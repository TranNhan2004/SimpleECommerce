using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Options;

using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Services.Keycloak;

public class KeycloakTokenService : IKeycloakTokenService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;

    public KeycloakTokenService(HttpClient httpClient, IOptions<KeycloakSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<KeycloakTokenResponse> GetTokenAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint);

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", _settings.Resource },
            { "client_secret", _settings.Credentials.Secret },
            { "username", username },
            { "password", password },
            { "scope", "openid profile email simple-e-commerce-roles" }
        };

        request.Content = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new UnauthorizedException($"Failed to authenticate: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (tokenResponse == null)
                throw new UnauthorizedException("Invalid token response from Keycloak");

            if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken) ||
                string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
                throw new UnauthorizedException("Token response from Keycloak was missing required fields");

            return new KeycloakTokenResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                RefreshExpiresIn = tokenResponse.RefreshExpiresIn,
                TokenType = tokenResponse.TokenType,
                Scope = tokenResponse.Scope
            };
        }
        catch (HttpRequestException ex)
        {
            throw new UnauthorizedException($"Failed to connect to Keycloak: {ex.Message}");
        }
    }

    public async Task<KeycloakTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint);

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", _settings.Resource },
            { "client_secret", _settings.Credentials.Secret },
            { "refresh_token", refreshToken }
        };

        request.Content = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedException("Invalid or expired refresh token");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new UnauthorizedException("Invalid token response from Keycloak");

            if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken) ||
                string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
                throw new UnauthorizedException("Token response from Keycloak was missing required fields");

            return new KeycloakTokenResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                RefreshExpiresIn = tokenResponse.RefreshExpiresIn,
                TokenType = tokenResponse.TokenType,
                Scope = tokenResponse.Scope
            };
        }
        catch (HttpRequestException ex)
        {
            throw new UnauthorizedException($"Failed to refresh token: {ex.Message}");
        }
    }

    public async Task<KeycloakUserInfoResponse> GetUserInfoAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _settings.UserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedException("Invalid access token");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var userInfo = JsonSerializer.Deserialize<KeycloakUserInfoDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new UnauthorizedException("Invalid user info response");

            return new KeycloakUserInfoResponse
            {
                Sub = userInfo.Sub,
                Email = userInfo.Email,
                EmailVerified = userInfo.EmailVerified,
                PreferredUsername = userInfo.PreferredUsername,
                GivenName = userInfo.GivenName ?? "",
                FamilyName = userInfo.FamilyName ?? "",
                Roles = userInfo.Roles ?? []
            };
        }
        catch (HttpRequestException ex)
        {
            throw new UnauthorizedException($"Failed to get user info: {ex.Message}");
        }
    }

    public async Task<bool> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.IntrospectionEndpoint);

        var parameters = new Dictionary<string, string>
        {
            { "client_id", _settings.Resource },
            { "client_secret", _settings.Credentials.Secret },
            { "token", accessToken }
        };

        request.Content = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var introspection = JsonSerializer.Deserialize<TokenIntrospectionDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return introspection?.Active ?? false;
        }
        catch
        {
            return false;
        }
    }

    // DTOs for deserialization
    private class KeycloakTokenResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = null!;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = null!;

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = null!;
    }

    private class KeycloakUserInfoDto
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("preferred_username")]
        public string PreferredUsername { get; set; } = null!;

        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }

        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }
    }

    private class TokenIntrospectionDto
    {
        public bool Active { get; set; }
    }
}
