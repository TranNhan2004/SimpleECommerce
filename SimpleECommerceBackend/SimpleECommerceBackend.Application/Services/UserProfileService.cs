using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Services;

[AutoConstructor]
public partial class UserProfileService : IUserProfileService
{
    private readonly ICacheService _cacheService;
    private readonly IUserProfileRepository _userProfileRepository;

    public async Task<UserProfile> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ResourceNotFoundException(
                UserProfileErrorCode.NotFoundById,
                $"User profile with Id = {id} not found."
            );

        var cachedProfile = await _cacheService.GetAsync<UserProfile>(UserProfileCacheKey.GetProfile.Replace("{id}", id.ToString()));
        if (cachedProfile != null) return cachedProfile;

        var userProfile = await _userProfileRepository.FindByIdAsync(id)
                          ?? throw new ResourceNotFoundException(
                              UserProfileErrorCode.NotFoundById,
                              $"User profile with Id = {id} not found."
                          );

        await _cacheService.SetAsync(
            UserProfileCacheKey.GetProfile.Replace("{id}", id.ToString()),
            userProfile,
            TimeSpan.FromMinutes(UserProfileCacheKey.GetProfileTtlMinutes)
        );
        return userProfile;
    }

    public async Task<UserProfile> GetByIdForUpdateAsync(Guid id)
    {
        return await _userProfileRepository.FindByIdAsync(id, true)
               ?? throw new ResourceNotFoundException(
                   UserProfileErrorCode.NotFoundById,
                   $"User profile with Id = {id} not found."
               );
    }

    public async Task InvalidateCacheAsync(Guid id)
    {
        await _cacheService.RemoveAsync(UserProfileCacheKey.GetProfile.Replace("{id}", id.ToString()));
    }
    
}