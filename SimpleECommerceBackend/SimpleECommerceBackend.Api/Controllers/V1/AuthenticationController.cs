using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Api.Dtos.V1.Auth;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Infrastructure.Options.Authentication;
using Microsoft.Extensions.Options;

namespace SimpleECommerceBackend.Api.Controllers.V1;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IUseCaseDispatcher _dispatcher;
    private readonly AuthOptions _authOptions;

    public AuthenticationController(IUseCaseDispatcher dispatcher, IOptions<AuthOptions> authOptions)
    {
        _dispatcher = dispatcher;
        _authOptions = authOptions.Value;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public IActionResult RegisterAsync()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpGet("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> LoginAsync([FromQuery] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = LoginRequest.ToCommand(request);
        var result = await _dispatcher.SendAsync<BeginLoginCommand, BeginLoginResult>(command, cancellationToken);
        return Redirect(result.AuthorizationUrl);
    }

    [HttpGet("login/callback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> LoginCallbackAsync([FromQuery] LoginCallbackRequest request, CancellationToken cancellationToken)
    {
        var command = LoginCallbackRequest.ToCommand(request);
        var result = await _dispatcher.SendAsync<CompleteLoginCommand, CompleteLoginResult>(command, cancellationToken);
        AppendSessionCookie(result.SessionId, result.SessionExpiresAtUtc);
        AppendCsrfCookie(result.CsrfToken, result.SessionExpiresAtUtc);
        return Redirect(result.FrontendRedirectUrl);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LogoutResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var command = new LogoutCommand
        {
            SessionId = Request.Cookies.TryGetValue(_authOptions.SessionCookieName, out var sessionId) ? sessionId : null,
            CsrfCookieToken = Request.Cookies.TryGetValue(_authOptions.CsrfCookieName, out var csrfCookieToken) ? csrfCookieToken : null,
            CsrfHeaderToken = Request.Headers.TryGetValue(_authOptions.CsrfHeaderName, out var csrfHeaderToken)
                ? csrfHeaderToken.ToString()
                : null
        };

        var result = await _dispatcher.SendAsync<LogoutCommand, LogoutResult>(command, cancellationToken);
        DeleteAuthCookies();
        return Ok(LogoutResponse.FromResult(result));
    }

    [HttpPost("refresh-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefreshTokenResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(_authOptions.SessionCookieName, out var sessionId)
            || string.IsNullOrWhiteSpace(sessionId))
        {
            throw new UnauthorizedException(AuthErrorCodes.SessionNotFound, "Authentication session was not found.");
        }

        var command = new RefreshTokenCommand
        {
            SessionId = sessionId,
            CsrfCookieToken = Request.Cookies.TryGetValue(_authOptions.CsrfCookieName, out var csrfCookieToken) ? csrfCookieToken : null,
            CsrfHeaderToken = Request.Headers.TryGetValue(_authOptions.CsrfHeaderName, out var csrfHeaderToken)
                ? csrfHeaderToken.ToString()
                : null
        };

        var result = await _dispatcher.SendAsync<RefreshTokenCommand, RefreshTokenResult>(command, cancellationToken);
        AppendSessionCookie(sessionId, result.SessionExpiresAtUtc);
        if (!string.IsNullOrWhiteSpace(command.CsrfCookieToken))
        {
            AppendCsrfCookie(command.CsrfCookieToken, result.SessionExpiresAtUtc);
        }

        return Ok(RefreshTokenResponse.FromResult(result));
    }

    private void AppendSessionCookie(string sessionId, DateTimeOffset expiresAtUtc)
    {
        Response.Cookies.Append(
            _authOptions.SessionCookieName,
            sessionId,
            BuildCookieOptions(expiresAtUtc, httpOnly: true)
        );
    }

    private void AppendCsrfCookie(string csrfToken, DateTimeOffset expiresAtUtc)
    {
        Response.Cookies.Append(
            _authOptions.CsrfCookieName,
            csrfToken,
            BuildCookieOptions(expiresAtUtc, httpOnly: false)
        );
    }

    private void DeleteAuthCookies()
    {
        Response.Cookies.Delete(_authOptions.SessionCookieName, BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(-1), httpOnly: true));
        Response.Cookies.Delete(_authOptions.CsrfCookieName, BuildCookieOptions(DateTimeOffset.UtcNow.AddDays(-1), httpOnly: false));
    }

    private CookieOptions BuildCookieOptions(DateTimeOffset expiresAtUtc, bool httpOnly)
    {
        return new CookieOptions
        {
            HttpOnly = httpOnly,
            Secure = _authOptions.CookieSecure,
            SameSite = ParseSameSiteMode(_authOptions.CookieSameSite),
            Expires = expiresAtUtc,
            IsEssential = true,
            Path = "/"
        };
    }

    private static SameSiteMode ParseSameSiteMode(string sameSiteMode)
    {
        return Enum.TryParse<SameSiteMode>(sameSiteMode, ignoreCase: true, out var parsedMode)
            ? parsedMode
            : SameSiteMode.Lax;
    }
}
