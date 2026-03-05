namespace SimpleECommerceBackend.Application.Models.Keycloak;

public class CreateKeycloakUserResponse
{
    public string KeycloakUserId { get; init; } = null!;
    public string Email { get; init; } = null!;
}