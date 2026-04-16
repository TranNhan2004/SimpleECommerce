using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface IUserProfileService : ICacheConsumingService
{
    UserProfile CreateUserProfile(UserProfile userProfile);
    Task<UserProfile> GetByIdForUpdateAsync(Guid id);
    Task<UserProfile> GetByIdAsync(Guid id);
    Task<bool> IsActiveUserAsync(Guid id);
}