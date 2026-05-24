using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.Services.Uam;

public class PermissionService : ServiceBase, IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(
        ICacheService cacheService,
        IPermissionRepository permissionRepository
    ) : base(cacheService)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = PermissionCacheKeys.GetPermissionSetKey(userId);
        var cachedPermissionCodes = await CacheService.GetAsync<List<string>>(cacheKey, cancellationToken);
        if (cachedPermissionCodes is not null)
            return cachedPermissionCodes;

        var permissionCodes = await _permissionRepository.FindCodesByUserIdAsync(userId);
        await CacheService.SetAsync(
            cacheKey,
            permissionCodes.ToList(),
            TimeSpan.FromMinutes(PermissionCacheKeys.PermissionSetTtlMinutes),
            cancellationToken
        );

        return permissionCodes;
    }
}
