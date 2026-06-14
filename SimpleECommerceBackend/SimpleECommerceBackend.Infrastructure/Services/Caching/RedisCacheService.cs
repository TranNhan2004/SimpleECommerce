using System.Text.Json;
using Microsoft.Extensions.Options;
using Serilog;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Infrastructure.Options.Caching;
using StackExchange.Redis;

namespace SimpleECommerceBackend.Infrastructure.Services.Caching;

public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly JsonSerializerOptions PrefixIndexJsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(2);

    private readonly RedisOptions _redisOptions;
    private readonly ILogger? _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private IConnectionMultiplexer? _connection;

    public RedisCacheService(IOptions<RedisOptions> redisOptions, ILogger logger)
    {
        _redisOptions = redisOptions.Value;
        _logger = logger;
    }

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisOptions> redisOptions)
    {
        _connection = connectionMultiplexer;
        _redisOptions = redisOptions.Value;
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        var database = await TryGetDatabaseAsync(cancellationToken);
        if (database is null)
            return null;

        try
        {
            var value = await database.StringGetAsync(BuildKey(key));
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            LogCacheFailure(ex, "read string", key);
            return null;
        }
    }

    public Task SetStringAsync(string key, string value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        return SetValueAsync(key, value, ttl, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var database = await TryGetDatabaseAsync(cancellationToken);
        if (database is null)
            return default;

        var redisKey = BuildKey(key);

        try
        {
            var value = await database.StringGetAsync(redisKey);
            if (!value.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
        }
        catch (Exception ex) when (IsInvalidCachedValue(ex))
        {
            await DeleteInvalidCacheEntryAsync(database, redisKey);
            return default;
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            LogCacheFailure(ex, "read", key);
            return default;
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        return SetValueAsync(key, json, ttl, cancellationToken);
    }

    public async Task<IEnumerable<T?>> GetBulkAsync<T>(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        if (keys.Count == 0)
            return [];

        var database = await TryGetDatabaseAsync(cancellationToken);
        if (database is null)
            return [.. keys.Select(_ => default(T?))];

        RedisKey[] redisKeys = [.. keys.Select(key => (RedisKey)BuildKey(key))];

        try
        {
            var values = await database.StringGetAsync(redisKeys);
            return await DeserializeBulkAsync<T>(database, redisKeys, values, cancellationToken);
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            _logger?.Warning(ex, "Redis cache failed to read {Count} keys.", keys.Count);
            return [.. keys.Select(_ => default(T?))];
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var database = await TryGetDatabaseAsync(cancellationToken);
        if (database is null)
            return;

        var redisKey = BuildKey(key);

        try
        {
            await database.KeyDeleteAsync(redisKey);
            await RemoveTrackedKeyAsync(database, BuildPrefixIndexKey(ExtractPrefix(key)), redisKey);
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            LogCacheFailure(ex, "remove", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        var database = await TryGetDatabaseAsync(cancellationToken);
        if (database is null)
            return;

        try
        {
            var pattern = BuildKey($"{prefix}*");
            foreach (var endpoint in database.Multiplexer.GetEndPoints())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var server = database.Multiplexer.GetServer(endpoint);
                if (!server.IsConnected)
                    continue;

                foreach (var key in server.Keys(database.Database, pattern: pattern))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await database.KeyDeleteAsync(key);
                }
            }

            await database.KeyDeleteAsync(BuildPrefixIndexKey(ExtractPrefix(prefix)));
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            _logger?.Warning(ex, "Redis cache failed to remove keys with prefix {Prefix}.", prefix);
        }
    }

    private async Task SetValueAsync(
        string key,
        RedisValue value,
        TimeSpan ttl,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var database = await TryGetDatabaseAsync(cancellationToken);
        if (database is null)
            return;

        try
        {
            var redisKey = BuildKey(key);
            await database.StringSetAsync(redisKey, value, ttl);
            await EnforcePrefixKeyLimitAsync(database, key, redisKey, cancellationToken);
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            LogCacheFailure(ex, "write", key);
        }
    }

    private async Task<T?[]> DeserializeBulkAsync<T>(
        IDatabase database,
        RedisKey[] redisKeys,
        RedisValue[] values,
        CancellationToken cancellationToken)
    {
        var results = new T?[values.Length];

        for (var index = 0; index < values.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!values[index].HasValue)
                continue;

            try
            {
                results[index] = JsonSerializer.Deserialize<T>(values[index].ToString(), JsonOptions);
            }
            catch (Exception ex) when (IsInvalidCachedValue(ex))
            {
                await DeleteInvalidCacheEntryAsync(database, redisKeys[index]);
            }
        }

        return results;
    }

    private async Task EnforcePrefixKeyLimitAsync(
        IDatabase database,
        string logicalKey,
        RedisKey redisKey,
        CancellationToken cancellationToken)
    {
        var prefix = ExtractPrefix(logicalKey);
        if (!_redisOptions.PrefixKeyLimits.TryGetValue(prefix, out var limit))
            return;

        var prefixIndexKey = BuildPrefixIndexKey(prefix);
        var trackedKeys = await GetTrackedKeysAsync(database, prefixIndexKey);

        await RemoveMissingTrackedKeysAsync(database, trackedKeys, cancellationToken);

        trackedKeys.Remove(redisKey!);
        trackedKeys.Add(redisKey!);

        while (trackedKeys.Count > limit)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var oldestKey = trackedKeys[0];
            trackedKeys.RemoveAt(0);
            await database.KeyDeleteAsync((RedisKey)oldestKey);
        }

        await SaveTrackedKeysAsync(database, prefixIndexKey, trackedKeys);
    }

    private static async Task RemoveMissingTrackedKeysAsync(
        IDatabase database,
        List<string> trackedKeys,
        CancellationToken cancellationToken)
    {
        for (var index = trackedKeys.Count - 1; index >= 0; index--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var exists = await database.KeyExistsAsync((RedisKey)trackedKeys[index]);
            if (!exists)
                trackedKeys.RemoveAt(index);
        }
    }

    private async Task RemoveTrackedKeyAsync(
        IDatabase database,
        string prefixIndexKey,
        string redisKey)
    {
        var trackedKeys = await GetTrackedKeysAsync(database, prefixIndexKey);
        if (!trackedKeys.Remove(redisKey))
            return;

        await SaveTrackedKeysAsync(database, prefixIndexKey, trackedKeys);
    }

    private async Task<List<string>> GetTrackedKeysAsync(IDatabase database, string prefixIndexKey)
    {
        var serializedTrackedKeys = await database.StringGetAsync(prefixIndexKey);
        if (!serializedTrackedKeys.HasValue)
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(
                serializedTrackedKeys.ToString(),
                PrefixIndexJsonOptions
            ) ?? [];
        }
        catch (JsonException)
        {
            await database.KeyDeleteAsync(prefixIndexKey);
            return [];
        }
    }

    private static async Task SaveTrackedKeysAsync(
        IDatabase database,
        string prefixIndexKey,
        List<string> trackedKeys)
    {
        if (trackedKeys.Count == 0)
        {
            await database.KeyDeleteAsync(prefixIndexKey);
            return;
        }

        await database.StringSetAsync(
            prefixIndexKey,
            JsonSerializer.Serialize(trackedKeys, PrefixIndexJsonOptions)
        );
    }

    private async Task<IDatabase?> TryGetDatabaseAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_connection?.IsConnected == true)
            return _connection.GetDatabase();

        if (!await _connectionLock.WaitAsync(ConnectTimeout, cancellationToken))
            return null;

        try
        {
            if (_connection?.IsConnected == true)
                return _connection.GetDatabase();

            _connection = await CreateConnectionAsync(cancellationToken);
            return _connection?.GetDatabase();
        }
        catch (Exception ex) when (IsCacheFailure(ex, cancellationToken))
        {
            _logger?.Warning(ex, "Redis cache is unavailable. Continuing without cache.");
            return null;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task<IConnectionMultiplexer?> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        var options = ConfigurationOptions.Parse(_redisOptions.ConnectionString);
        options.AbortOnConnectFail = false;
        options.ConnectTimeout = (int)ConnectTimeout.TotalMilliseconds;
        options.SyncTimeout = (int)ConnectTimeout.TotalMilliseconds;
        options.AsyncTimeout = (int)ConnectTimeout.TotalMilliseconds;

        return await ConnectionMultiplexer
            .ConnectAsync(options)
            .WaitAsync(ConnectTimeout, cancellationToken);
    }

    private static async Task DeleteInvalidCacheEntryAsync(IDatabase database, RedisKey redisKey)
    {
        await database.KeyDeleteAsync(redisKey);
    }

    private static bool IsInvalidCachedValue(Exception exception)
    {
        return exception is JsonException or NotSupportedException or ValidationException;
    }

    private static bool IsCacheFailure(Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && cancellationToken.IsCancellationRequested)
            return false;

        return exception is RedisException
            or TimeoutException
            or ObjectDisposedException
            or OperationCanceledException;
    }

    private void LogCacheFailure(Exception exception, string operation, string key)
    {
        _logger?.Warning(exception, "Redis cache failed to {Operation} key {CacheKey}.", operation, key);
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
