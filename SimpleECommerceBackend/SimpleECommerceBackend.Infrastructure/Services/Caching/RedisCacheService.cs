using System.Text.Json;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Infrastructure.Options.Caching;
using StackExchange.Redis;

namespace SimpleECommerceBackend.Infrastructure.Services.Caching;

public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly JsonSerializerOptions PrefixIndexJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDatabase _database;
    private readonly RedisOptions _redisOptions;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisOptions> redisOptions)
    {
        _database = connectionMultiplexer.GetDatabase();
        _redisOptions = redisOptions.Value;
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var value = await _database.StringGetAsync(BuildKey(key));
        return value.HasValue ? value.ToString() : null;
    }

    public Task SetStringAsync(string key, string value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        return SetValueAsync(key, value, ttl, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var redisKey = BuildKey(key);
        var value = await _database.StringGetAsync(redisKey);
        if (!value.HasValue)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException or ValidationException)
        {
            await _database.KeyDeleteAsync(redisKey);
            return default;
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        return SetValueAsync(key, json, ttl, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var redisKey = BuildKey(key);
        await _database.KeyDeleteAsync(redisKey);
        await RemoveTrackedKeyAsync(BuildPrefixIndexKey(ExtractPrefix(key)), redisKey);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        cancellationToken.ThrowIfCancellationRequested();

        var pattern = BuildKey($"{prefix}*");

        foreach (var endpoint in _database.Multiplexer.GetEndPoints())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var server = _database.Multiplexer.GetServer(endpoint);
            if (!server.IsConnected)
            {
                continue;
            }

            foreach (var key in server.Keys(_database.Database, pattern: pattern))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _database.KeyDeleteAsync(key);
            }
        }

        await _database.KeyDeleteAsync(BuildPrefixIndexKey(ExtractPrefix(prefix)));
    }

    public async Task<IEnumerable<T?>> GetBulkAsync<T>(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();

        if (keys.Count == 0)
            return [];

        RedisKey[] redisKeys = [.. keys.Select(k => (RedisKey)BuildKey(k))];

        RedisValue[] values = await _database.StringGetAsync(redisKeys);

        var results = new T?[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!values[i].HasValue)
            {
                results[i] = default;
                continue;
            }

            try
            {
                results[i] = JsonSerializer.Deserialize<T>(values[i].ToString(), JsonOptions);
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException or ValidationException)
            {
                await _database.KeyDeleteAsync(redisKeys[i]);
                results[i] = default;
            }
        }

        return results;
    }

    private async Task SetValueAsync(
        string key,
        RedisValue value,
        TimeSpan ttl,
        CancellationToken cancellationToken
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        cancellationToken.ThrowIfCancellationRequested();

        var redisKey = BuildKey(key);
        await _database.StringSetAsync(redisKey, value, ttl);
        await EnforcePrefixKeyLimitAsync(key, redisKey, cancellationToken);
    }

    private async Task EnforcePrefixKeyLimitAsync(
        string logicalKey,
        string redisKey,
        CancellationToken cancellationToken
    )
    {
        var prefix = ExtractPrefix(logicalKey);
        if (!_redisOptions.PrefixKeyLimits.TryGetValue(prefix, out var prefixKeyLimit))
        {
            return;
        }

        var prefixIndexKey = BuildPrefixIndexKey(prefix);
        var trackedKeys = await GetTrackedKeysAsync(prefixIndexKey, cancellationToken);

        for (var index = trackedKeys.Count - 1; index >= 0; index--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var exists = await _database.KeyExistsAsync((RedisKey)trackedKeys[index]);
            if (!exists)
            {
                trackedKeys.RemoveAt(index);
            }
        }

        trackedKeys.Remove(redisKey);
        trackedKeys.Add(redisKey);

        while (trackedKeys.Count > prefixKeyLimit)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var oldestKey = trackedKeys[0];
            trackedKeys.RemoveAt(0);
            await _database.KeyDeleteAsync((RedisKey)oldestKey);
        }

        await SaveTrackedKeysAsync(prefixIndexKey, trackedKeys);
    }

    private async Task RemoveTrackedKeyAsync(
        string prefixIndexKey,
        string redisKey
    )
    {
        var trackedKeys = await GetTrackedKeysAsync(prefixIndexKey, CancellationToken.None);
        if (!trackedKeys.Remove(redisKey))
        {
            return;
        }

        await SaveTrackedKeysAsync(prefixIndexKey, trackedKeys);
    }

    private async Task<List<string>> GetTrackedKeysAsync(
        string prefixIndexKey,
        CancellationToken cancellationToken
    )
    {
        var serializedTrackedKeys = await _database.StringGetAsync(prefixIndexKey);
        if (!serializedTrackedKeys.HasValue)
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(
                serializedTrackedKeys.ToString(),
                PrefixIndexJsonOptions
            ) ?? [];
        }
        catch (JsonException)
        {
            await _database.KeyDeleteAsync(prefixIndexKey);
            return [];
        }
    }

    private async Task SaveTrackedKeysAsync(string prefixIndexKey, List<string> trackedKeys)
    {
        if (trackedKeys.Count == 0)
        {
            await _database.KeyDeleteAsync(prefixIndexKey);
            return;
        }

        await _database.StringSetAsync(
            prefixIndexKey,
            JsonSerializer.Serialize(trackedKeys, PrefixIndexJsonOptions)
        );
    }

    public static string ExtractPrefix(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var normalizedKey = key.Trim();
        var separatorIndex = normalizedKey.IndexOf(':');

        return separatorIndex < 0
            ? normalizedKey
            : normalizedKey[..separatorIndex];
    }

    private string BuildPrefixIndexKey(string prefix) => BuildKey($"__prefix_index__:{prefix}");

    private string BuildKey(string key) => $"{_redisOptions.InstanceName}{key}";
}
