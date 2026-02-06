using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> FindAllAsync();
    Task<Role?> FindByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Role Add(Role role);
    Role Update(Role role);
    Role Delete(Role role);
}