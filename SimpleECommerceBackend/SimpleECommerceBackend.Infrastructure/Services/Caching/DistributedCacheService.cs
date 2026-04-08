using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;

namespace SimpleECommerceBackend.Infrastructure.Services.Caching;

[AutoConstructor]
public partial class DistributedCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDistributedCache _cache;

    public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.GetStringAsync(key, cancellationToken);
    }

    public Task SetStringAsync(string key, string value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        return _cache.SetStringAsync(key, value, CreateOptions(ttl), cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        return _cache.SetStringAsync(key, json, CreateOptions(ttl), cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }

    private static DistributedCacheEntryOptions CreateOptions(TimeSpan ttl)
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };
    }
}
