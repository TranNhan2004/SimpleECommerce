namespace SimpleECommerceBackend.Application.Models.Keycloak;

public class KeycloakUserInfoResponse
{
    public string Sub { get; init; } = null!;
    public string Email { get; init; } = null!;
    public bool EmailVerified { get; init; }
    public string PreferredUsername { get; init; } = null!;
    public string GivenName { get; init; } = null!;
    public string FamilyName { get; init; } = null!;
    public List<string> Roles { get; init; } = [];
}