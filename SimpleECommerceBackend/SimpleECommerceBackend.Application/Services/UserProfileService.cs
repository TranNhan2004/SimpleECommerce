using Serilog;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Services;

public class UserProfileService : ServiceBase, IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger _logger;

    public UserProfileService(
        ICacheService cacheService,
        IUserProfileRepository userProfileRepository,
        ILogger logger
    ) : base(cacheService)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public UserProfile CreateUserProfile(UserProfile userProfile)
    {
        return _userProfileRepository.Add(userProfile);
    }

    public async Task<UserProfile> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ResourceNotFoundException(
                UserProfileErrorCodes.NotFoundById,
                $"User profile with Id = {id} not found."
            );

        var cacheKey = UserProfileCacheKeys.GetProfileKey(id);
        var cachedProfile = await CacheService.GetAsync<UserProfile>(cacheKey);

        if (cachedProfile is not null)
        {
            _logger.Debug("Cache hit for user profile {CacheKey}", cacheKey);
            return cachedProfile;
        }

        _logger.Debug("Cache miss for user profile {CacheKey}", cacheKey);

        var userProfile = await _userProfileRepository.FindByIdAsync(id)
                          ?? throw new ResourceNotFoundException(
                              UserProfileErrorCodes.NotFoundById,
                              $"User profile with Id = {id} not found."
                          );

        await CacheService.SetAsync(
            cacheKey,
            userProfile,
            TimeSpan.FromMinutes(UserProfileCacheKeys.GetProfileTtlMinutes)
        );
        return userProfile;
    }

    public async Task<UserProfile> GetByIdForUpdateAsync(Guid id)
    {
        return await _userProfileRepository.FindByIdAsync(id, true)
               ?? throw new ResourceNotFoundException(
                   UserProfileErrorCodes.NotFoundById,
                   $"User profile with Id = {id} not found."
               );
    }

    public async Task<bool> IsActiveUserAsync(Guid id)
    {
        var userProfile = await GetByIdAsync(id);
        return userProfile is not null && userProfile.Status == UserStatus.Active;
    }
}
