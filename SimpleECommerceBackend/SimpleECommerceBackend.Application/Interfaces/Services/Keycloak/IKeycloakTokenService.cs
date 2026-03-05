using SimpleECommerceBackend.Application.Models.Keycloak;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;

public interface IKeycloakTokenService
{
    Task<KeycloakTokenResponse> GetTokenAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<KeycloakTokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<KeycloakUserInfoResponse> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}
