namespace SimpleECommerceBackend.Infrastructure.Options.Authentication;

public class AuthOptions
{
    public const string SectionName = "Auth";

    public string FrontendBaseUrl { get; init; } = null!;
    public string PostLoginRedirectPath { get; init; } = "/";
    public string PostLogoutRedirectPath { get; init; } = "/login";
    public string SessionCookieName { get; init; } = "APP_SESSION_ID";
    public string CsrfCookieName { get; init; } = "XSRF-TOKEN";
    public string CsrfHeaderName { get; init; } = "X-CSRF-TOKEN";
    public int SessionExpireMinutes { get; init; }
    public int OAuthStateExpireMinutes { get; init; }
    public bool CookieSecure { get; init; }
    public string CookieSameSite { get; init; } = "Lax";
}
