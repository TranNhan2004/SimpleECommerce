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

        // Run prefix invalidation in background to avoid blocking the request
        // (RemoveByPrefixAsync can be expensive for large keyspaces).
        foreach (var prefix in prefixSet)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await CacheService.RemoveByPrefixAsync(prefix);
                }
                catch
                {
                    // Swallow exceptions to keep the request path resilient.
                }
            });
        }
    }

}