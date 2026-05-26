using SimpleECommerceBackend.Domain.Entities.Uam;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> FindByCodeAsync(string code, bool trackChanges = false);
}
