using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Infrastructure.Options.Caching;
using SimpleECommerceBackend.Infrastructure.Services.Caching;
using StackExchange.Redis;

namespace SimpleECommerceBackend.Infrastructure.Tests.Services.Caching;

public class RedisCacheServiceTests
{
    [Fact]
    public async Task SetStringAsync_ShouldStoreAndReturnStringValue()
    {
        var sut = CreateSut();

        await sut.SetStringAsync("cache:string", "hello", TimeSpan.FromMinutes(5));
        var result = await sut.GetStringAsync("cache:string");

        result.Should().Be("hello");
    }

    [Fact]
    public async Task SetAsync_ShouldStoreAndReturnTypedValue()
    {
        var sut = CreateSut();
        var expected = new CachePayload("translation", 2);

        await sut.SetAsync("cache:object", expected, TimeSpan.FromMinutes(5));
        var result = await sut.GetAsync<CachePayload>("cache:object");

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteExistingValue()
    {
        var sut = CreateSut();
        await sut.SetStringAsync("cache:remove", "value", TimeSpan.FromMinutes(5));

        await sut.RemoveAsync("cache:remove");
        var result = await sut.GetStringAsync("cache:remove");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDefaultAndRemoveKey_WhenPayloadCannotBeDeserialized()
    {
        var sut = CreateSut(out var storage);
        storage["test:cache:invalid-profile"] = """{"id":"00000000-0000-0000-0000-000000000001","email":"bad@example.com"}""";

        var result = await sut.GetAsync<SimpleECommerceBackend.Domain.Entities.Business.UserProfile>("cache:invalid-profile");

        result.Should().BeNull();
        storage.Should().NotContainKey("test:cache:invalid-profile");
    }

    [Fact]
    public async Task GetBulkAsync_ShouldTreatInvalidPayloadAsCacheMiss_AndRemoveBrokenKey()
    {
        var sut = CreateSut(out var storage);
        storage["test:cache:invalid-category"] = """{"id":"00000000-0000-0000-0000-000000000001","name":"Category A"}""";

        var result = (await sut.GetBulkAsync<SimpleECommerceBackend.Domain.Entities.Business.Category>(
            ["cache:missing-category", "cache:invalid-category"]))
            .ToList();

        result.Should().HaveCount(2);
        result[0].Should().BeNull();
        result[1].Should().BeNull();
        storage.Should().NotContainKey("test:cache:invalid-category");
    }

    private static RedisCacheService CreateSut()
    {
        return CreateSut(out _);
    }

    private static RedisCacheService CreateSut(out Dictionary<string, RedisValue> storage)
    {
        var cacheStorage = new Dictionary<string, RedisValue>();
        storage = cacheStorage;
        var databaseMock = new Mock<IDatabase>();

        databaseMock
            .Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
                cacheStorage.TryGetValue(key!, out var value) ? value : RedisValue.Null);

        databaseMock
            .Setup(x => x.StringGetAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) =>
                keys.Select(key => cacheStorage.TryGetValue(key!, out var value) ? value : RedisValue.Null).ToArray());

        databaseMock
            .Setup(x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, RedisValue value, TimeSpan? _, bool _, When _, CommandFlags _) =>
            {
                cacheStorage[key!] = value;
                return true;
            });

        databaseMock
            .Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) => cacheStorage.Remove(key!));

        var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
        connectionMultiplexerMock
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(databaseMock.Object);

        return new RedisCacheService(
            connectionMultiplexerMock.Object,
            Microsoft.Extensions.Options.Options.Create(
                new RedisOptions
                {
                    ConnectionString = "localhost:6379",
                    InstanceName = "test:"
                }
            )
        );
    }

    private sealed record CachePayload(string Name, int Count);
}
