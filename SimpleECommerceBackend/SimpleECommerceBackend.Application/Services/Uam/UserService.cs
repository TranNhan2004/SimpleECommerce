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

        var cacheKey = UserCacheKeys.GetUserByIdKey(id);
        var cachedUser = await CacheService.GetAsync<User>(cacheKey);

        if (cachedUser is not null)
        {
            return cachedUser;
        }

        var user = await _userRepository.FindByIdAsync(id)
                   ?? throw new ResourceNotFoundException(
                       UserProfileErrorCodes.NotFoundById,
                       $"User with Id = {id} not found."
                   );

        await CacheService.SetAsync(
            cacheKey,
            user,
            TimeSpan.FromMinutes(UserCacheKeys.GetUserByIdTtlMinutes)
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

    public async Task<Guid> GetIdByKeycloakSubjectIdAsync(Guid keycloakSubjectId)
    {
        if (keycloakSubjectId == Guid.Empty)
            throw new ResourceNotFoundException(
                UserProfileErrorCodes.NotFoundById,
                $"User with Keycloak Subject Id = {keycloakSubjectId} not found."
            );

        var cacheKey = UserCacheKeys.GetUserByKeycloakSubjectIdKey(keycloakSubjectId);
        var cachedUserId = await CacheService.GetAsync<string>(cacheKey);

        if (cachedUserId is not null)
        {
            try
            {
                return Guid.Parse(cachedUserId);
            }
            catch (FormatException ex)
            {
                _logger.Error(ex, "Cached user id for Keycloak Subject Id = {KeycloakSubjectId} is invalid: {CachedUserId}", keycloakSubjectId, cachedUserId);
                await CacheService.RemoveAsync(cacheKey);
                throw new ResourceNotFoundException(
                    UserProfileErrorCodes.NotFoundByKeycloakSubjectId,
                    $"User with Keycloak Subject Id = {keycloakSubjectId} not found."
                );
            }
        }

        var userId = await _userRepository.FindIdByKeycloakSubjectIdAsync(keycloakSubjectId)
                   ?? throw new ResourceNotFoundException(
                       UserProfileErrorCodes.NotFoundByKeycloakSubjectId,
                       $"User with Keycloak Subject Id = {keycloakSubjectId} not found."
                   );

        await CacheService.SetAsync(
            cacheKey,
            userId.ToString(),
            TimeSpan.FromMinutes(UserCacheKeys.GetUserByKeycloakSubjectIdTtlMinutes)
        );

        return userId;
    }

    public async Task<bool> IsActiveUserAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        return user.Status == UserStatus.Active;
    }
}
