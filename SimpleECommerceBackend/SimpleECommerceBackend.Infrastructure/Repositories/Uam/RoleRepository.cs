using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Uam;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<Role?> FindByCodeAsync(string code, bool trackChanges = false)
    {
        return await FindFirstByConditionAsync(
            query => query.Where(role => role.Code == code),
            trackChanges
        );
    }
}
