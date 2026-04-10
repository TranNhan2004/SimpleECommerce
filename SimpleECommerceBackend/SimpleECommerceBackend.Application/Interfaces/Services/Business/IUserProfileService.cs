using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface IUserProfileService
{
    Task<UserProfile> GetByIdForUpdateAsync(Guid id);
    Task<UserProfile> GetByIdAsync(Guid id);

    Task InvalidateCacheAsync(Guid id);
}