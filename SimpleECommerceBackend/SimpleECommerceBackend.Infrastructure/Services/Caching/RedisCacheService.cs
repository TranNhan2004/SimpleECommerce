using System.Text.Json;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Infrastructure.Options.Caching;
using StackExchange.Redis;

namespace SimpleECommerceBackend.Infrastructure.Services.Caching;

public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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
        cancellationToken.ThrowIfCancellationRequested();
        return _database.StringSetAsync(BuildKey(key), value, ttl);
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
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            await _database.KeyDeleteAsync(redisKey);
            return default;
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = JsonSerializer.Serialize(value, JsonOptions);
        return _database.StringSetAsync(BuildKey(key), json, ttl);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _database.KeyDeleteAsync(BuildKey(key));
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
            catch (Exception ex) when (ex is JsonException or NotSupportedException)
            {
                await _database.KeyDeleteAsync(redisKeys[i]);
                results[i] = default;
            }
        }

        return results;
    }

    private string BuildKey(string key) => $"{_redisOptions.InstanceName}{key}";
}
