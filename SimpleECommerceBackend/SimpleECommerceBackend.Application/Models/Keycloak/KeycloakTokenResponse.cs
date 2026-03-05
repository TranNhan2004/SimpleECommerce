namespace SimpleECommerceBackend.Application.Models.Keycloak;

public class KeycloakTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
    public int RefreshExpiresIn { get; init; }
    public string TokenType { get; init; } = null!;
    public string Scope { get; init; } = null!;
}

