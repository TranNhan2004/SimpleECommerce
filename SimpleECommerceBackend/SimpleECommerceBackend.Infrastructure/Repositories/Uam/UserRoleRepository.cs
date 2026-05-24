using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Uam;

public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid roleId, bool trackChanges = false)
    {
        var query = QueryAll(trackChanges);
        return await query.AnyAsync(userRole => userRole.UserId == userId && userRole.RoleId == roleId);
    }
}
