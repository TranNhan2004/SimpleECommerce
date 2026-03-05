using SimpleECommerceBackend.Application.Models.Keycloak;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;

public interface IKeycloakAdminService
{
    Task<CreateKeycloakUserResponse> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
}
