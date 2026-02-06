using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> FindAllAsync();
    Task<Permission?> FindByIdAsync(Guid id);
    Permission Add(Permission permission);
    Permission Update(Permission permission);
    Permission Delete(Permission permission);
}