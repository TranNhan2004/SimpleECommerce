using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.Services.Uam;

public class PermissionService : IPermissionService
{
    private readonly Serilog.ILogger _logger;
    private readonly ICacheService _cacheService;
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(
        Serilog.ILogger logger,
        ICacheService cacheService,
        IPermissionRepository permissionRepository
    )
    {
        _logger = logger;
        _cacheService = cacheService;
        _permissionRepository = permissionRepository;
    }

    public async Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = PermissionCacheKeys.GetPermissionSetKey(userId);
        var cachedPermissionCodes = await _cacheService.GetAsync<List<string>>(cacheKey, cancellationToken);
        if (cachedPermissionCodes is not null)
            return cachedPermissionCodes;

        var permissionCodes = await _permissionRepository.FindCodesByUserIdAsync(userId);
        await _cacheService.SetAsync(
            cacheKey,
            permissionCodes.ToList(),
            TimeSpan.FromMinutes(PermissionCacheKeys.PermissionSetTtlMinutes),
            cancellationToken
        );

        return permissionCodes;
    }
}
