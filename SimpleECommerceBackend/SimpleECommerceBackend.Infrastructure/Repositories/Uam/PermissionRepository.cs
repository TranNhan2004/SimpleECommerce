using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Uam;

public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<IReadOnlyList<string>> FindCodesByUserIdAsync(Guid userId, bool trackChanges = false)
    {
        IQueryable<Permission> query = DbContext.Set<Permission>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .Where(permission => DbContext.Set<RolePermission>().Any(rolePermission =>
                rolePermission.PermissionId == permission.Id &&
                    DbContext.Set<UserRole>().Any(userRole =>
                    userRole.UserId == userId &&
                    userRole.RoleId == rolePermission.RoleId
                )
            ))
            .Select(permission => permission.Code)
            .Distinct()
            .OrderBy(code => code)
            .ToListAsync();
    }
}
