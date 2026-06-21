using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Entities.Uam;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;

public interface IUserRepository : IGenericRepository<User>
{
    Task<Guid?> FindIdByKeycloakSubjectIdAsync(Guid keycloakSubjectId);
    Task<GetMeResult?> FindWithPermissionsByIdAsync(Guid userId, bool trackChanges = false);
}
