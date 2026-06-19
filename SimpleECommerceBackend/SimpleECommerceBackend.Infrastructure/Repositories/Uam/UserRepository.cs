using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Uam;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<Guid?> FindIdByKeycloakSubjectIdAsync(Guid keycloakSubjectId)
    {
        return await DbContext.Set<User>()
            .Where(u => u.KeycloakSubjectId == keycloakSubjectId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();
    }
}
