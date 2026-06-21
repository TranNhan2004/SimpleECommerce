using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Api.Dtos.V1.Auth;

public class LogoutResponse
{
    public string FrontendRedirectUrl { get; init; } = null!;
    public string? IdentityProviderLogoutUrl { get; init; }

    public static LogoutResponse FromResult(LogoutResult result)
    {
        return new LogoutResponse
        {
            FrontendRedirectUrl = result.FrontendRedirectUrl,
            IdentityProviderLogoutUrl = result.IdentityProviderLogoutUrl
        };
    }
}
