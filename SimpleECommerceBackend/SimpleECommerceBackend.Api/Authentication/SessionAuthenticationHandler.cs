using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Auth;
using SimpleECommerceBackend.Infrastructure.Options.Authentication;

namespace SimpleECommerceBackend.Api.Authentication;

public sealed class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IBffAuthenticationService _bffAuthenticationService;
    private readonly AuthOptions _authOptions;

    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IBffAuthenticationService bffAuthenticationService,
        IOptions<AuthOptions> authOptions)
        : base(options, logger, encoder)
    {
        _bffAuthenticationService = bffAuthenticationService;
        _authOptions = authOptions.Value;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue(_authOptions.SessionCookieName, out var sessionId)
            || string.IsNullOrWhiteSpace(sessionId))
        {
            return AuthenticateResult.NoResult();
        }

        var session = await _bffAuthenticationService.GetSessionAsync(sessionId, Context.RequestAborted);
        if (session is null)
        {
            return AuthenticateResult.Fail("Authentication session was not found.");
        }

        var claims = new List<Claim>
        {
            new("sub", session.Subject),
            new(ClaimTypes.NameIdentifier, session.Subject),
            new("preferred_username", session.PreferredUsername),
            new(ClaimTypes.Email, session.Email),
            new("email", session.Email),
            new(AppAuthenticationDefaults.SessionIdClaimType, session.SessionId)
        };

        var identity = new ClaimsIdentity(claims, AppAuthenticationDefaults.SessionScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
