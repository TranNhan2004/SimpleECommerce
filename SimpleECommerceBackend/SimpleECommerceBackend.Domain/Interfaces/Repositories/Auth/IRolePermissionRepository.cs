using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

public interface IRolePermissionRepository
{
    Task<IEnumerable<RolePermission>> FindAllAsync();
    Task<RolePermission?> FindByIdAsync(Guid id);
    RolePermission Add(RolePermission rolePermission);
    RolePermission Update(RolePermission rolePermission);
    RolePermission Delete(RolePermission rolePermission);
}