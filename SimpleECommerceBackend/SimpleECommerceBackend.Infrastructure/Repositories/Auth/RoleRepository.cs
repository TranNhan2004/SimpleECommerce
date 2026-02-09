using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Auth;

[AutoConstructor]
public sealed partial class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _db;

    public async Task<IEnumerable<Role>> FindAllAsync()
    {
        return await _db.Roles.ToListAsync();
    }

    public async Task<Role?> FindByIdAsync(Guid id)
    {
        return await _db.Roles.FindAsync(id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _db.Roles
            .AsNoTracking()
            .AnyAsync(r => r.Name == name);
    }

    public Role Add(Role role)
    {
        _db.Roles.Add(role);
        return role;
    }

    public Role Update(Role role)
    {
        _db.Roles.Update(role);
        return role;
    }

    public Role Delete(Role role)
    {
        _db.Roles.Remove(role);
        return role;
    }
}