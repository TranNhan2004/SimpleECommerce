using Serilog;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Services.Uam;

public class UserService : ServiceBase, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public UserService(
        ICacheService cacheService,
        IUserRepository userRepository,
        ILogger logger
    ) : base(cacheService)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public User CreateUser(User user)
    {
        return _userRepository.Add(user);
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ResourceNotFoundException(
                UserProfileErrorCodes.NotFoundById,
                $"User with Id = {id} not found."
            );

        var cacheKey = UserCacheKeys.GetProfileKey(id);
        var cachedUser = await CacheService.GetAsync<User>(cacheKey);

        if (cachedUser is not null)
        {
            _logger.Debug("Cache hit for user {CacheKey}", cacheKey);
            return cachedUser;
        }

        _logger.Debug("Cache miss for user {CacheKey}", cacheKey);

        var user = await _userRepository.FindByIdAsync(id)
                   ?? throw new ResourceNotFoundException(
                       UserProfileErrorCodes.NotFoundById,
                       $"User with Id = {id} not found."
                   );

        await CacheService.SetAsync(
            cacheKey,
            user,
            TimeSpan.FromMinutes(UserCacheKeys.GetProfileTtlMinutes)
        );

        return user;
    }

    public async Task<User> GetByIdForUpdateAsync(Guid id)
    {
        return await _userRepository.FindByIdAsync(id, true)
               ?? throw new ResourceNotFoundException(
                   UserProfileErrorCodes.NotFoundById,
                   $"User with Id = {id} not found."
               );
    }

    public async Task<bool> IsActiveUserAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        return user.Status == UserStatus.Active;
    }
}
