using Microsoft.Extensions.Caching.Distributed;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

[AutoConstructor]
public partial class DistributedTranslationCache : ITranslationCache
{
    private readonly IDistributedCache _cache;

    public Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.GetStringAsync(key, cancellationToken);
    }

    public Task SetAsync(string key, string value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        return _cache.SetStringAsync(key, value, options, cancellationToken);
    }
}
