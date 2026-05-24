using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Entities.Uam;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface IUserService : ICacheConsumingService
{
    User CreateUser(User user);
    Task<User> GetByIdForUpdateAsync(Guid id);
    Task<User> GetByIdAsync(Guid id);
    Task<bool> IsActiveUserAsync(Guid id);
}
