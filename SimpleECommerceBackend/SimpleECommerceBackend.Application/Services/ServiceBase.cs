using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;

namespace SimpleECommerceBackend.Application.Services;

public abstract class ServiceBase : ICacheConsumingService
{
    protected ICacheService CacheService { get; }

    protected ServiceBase(ICacheService cacheService)
    {
        CacheService = cacheService;
    }

    public virtual async Task InvalidateCacheAsync(
        List<string>? exactKeys = null,
        List<string>? prefixKeys = null
    )
    {
        var keysToRemove = new HashSet<string>(StringComparer.Ordinal);

        if (exactKeys is not null)
        {
            foreach (var key in exactKeys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    keysToRemove.Add(key.Trim());
                }
            }
        }

        foreach (var key in keysToRemove)
        {
            await CacheService.RemoveAsync(key);
        }

        if (prefixKeys is null)
        {
            return;
        }

        var prefixSet = new HashSet<string>(StringComparer.Ordinal);
        foreach (var prefix in prefixKeys)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                prefixSet.Add(prefix.Trim());
            }
        }

        foreach (var prefix in prefixSet)
        {
            await CacheService.RemoveByPrefixAsync(prefix);
        }
    }

}