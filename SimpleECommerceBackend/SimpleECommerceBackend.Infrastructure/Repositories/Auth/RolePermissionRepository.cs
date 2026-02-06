using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Infrastructure.Persistence.AppDbContext;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Auth;

[AutoConstructor]
public sealed partial class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AppDbContext _db;

    public async Task<IEnumerable<RolePermission>> FindAllAsync()
    {
        return await _db.RolePermissions.ToListAsync();
    }

    public async Task<RolePermission?> FindByIdAsync(Guid id)
    {
        return await _db.RolePermissions.FindAsync(id);
    }

    public RolePermission Add(RolePermission rolePermission)
    {
        _db.RolePermissions.Add(rolePermission);
        return rolePermission;
    }

    public RolePermission Update(RolePermission rolePermission)
    {
        _db.RolePermissions.Update(rolePermission);
        return rolePermission;
    }

    public RolePermission Delete(RolePermission rolePermission)
    {
        _db.RolePermissions.Remove(rolePermission);
        return rolePermission;
    }
}