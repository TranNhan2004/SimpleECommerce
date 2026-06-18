using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Services.Uam;

public class UserService : IUserService
{
    private readonly Serilog.ILogger _logger;
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;

    public UserService(
        Serilog.ILogger logger,
        ICacheService cacheService,
        IUserRepository userRepository
    )
    {
        _logger = logger;
        _cacheService = cacheService;
        _userRepository = userRepository;
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
        var cachedUser = await _cacheService.GetAsync<User>(cacheKey);

        if (cachedUser is not null)
        {
            return cachedUser;
        }

        var user = await _userRepository.FindByIdAsync(id)
                   ?? throw new ResourceNotFoundException(
                       UserProfileErrorCodes.NotFoundById,
                       $"User with Id = {id} not found."
                   );

        await _cacheService.SetAsync(
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
        var cachedUserId = await _cacheService.GetAsync<string>(cacheKey);

        if (cachedUserId is not null)
        {
            try
            {
                return Guid.Parse(cachedUserId);
            }
            catch (FormatException ex)
            {
                _logger.Error(ex, "Cached user id for Keycloak Subject Id = {KeycloakSubjectId} is invalid: {CachedUserId}", keycloakSubjectId, cachedUserId);
                await _cacheService.RemoveAsync(cacheKey);
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

        await _cacheService.SetAsync(
            cacheKey,
            userId.ToString(),
            TimeSpan.FromMinutes(UserCacheKeys.GetUserByKeycloakSubjectIdTtlMinutes)
        );

        return userId;
    }

    public async Task<bool> IsActiveUserAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        return !user.IsActive;
    }
}
