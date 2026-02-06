using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Infrastructure.Persistence.AppDbContext;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Auth;

[AutoConstructor]
public sealed partial class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _db;

    public async Task<IEnumerable<Permission>> FindAllAsync()
    {
        return await _db.Permissions.ToListAsync();
    }

    public async Task<Permission?> FindByIdAsync(Guid id)
    {
        return await _db.Permissions.FindAsync(id);
    }

    public Permission Add(Permission permission)
    {
        _db.Permissions.Add(permission);
        return permission;
    }

    public Permission Update(Permission permission)
    {
        _db.Permissions.Update(permission);
        return permission;
    }

    public Permission Delete(Permission permission)
    {
        _db.Permissions.Remove(permission);
        return permission;
    }
}