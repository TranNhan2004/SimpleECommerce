using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;

public interface IKeycloakAdminService
{
    Task<CreateKeycloakUserResponse> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(string userId, Role role, CancellationToken cancellationToken = default);
    Task MarkEmailAsVerifiedAsync(string userId, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
}
