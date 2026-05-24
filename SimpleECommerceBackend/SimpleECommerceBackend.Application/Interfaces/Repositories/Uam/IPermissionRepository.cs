using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Domain.Entities.Uam;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;

public interface IPermissionRepository : IGenericRepository<Permission>
{
    Task<IReadOnlyList<string>> FindCodesByUserIdAsync(Guid userId, bool trackChanges = false);
}
