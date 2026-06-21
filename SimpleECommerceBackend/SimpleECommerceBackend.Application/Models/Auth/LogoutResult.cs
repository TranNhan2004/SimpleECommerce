namespace SimpleECommerceBackend.Application.Models.Auth;

public class LogoutResult
{
    public string FrontendRedirectUrl { get; init; } = null!;
    public string? IdentityProviderLogoutUrl { get; init; }
}
