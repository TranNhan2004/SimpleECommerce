using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;
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
    public async Task SetAsync_ShouldStoreAndReturnUser()
    {
        var sut = CreateSut();
        var expected = new User
        {
            Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Email = "user@example.com",
            FirstName = "Nhan",
            LastName = "Tran",
            NickName = "nhan",
            Sex = Sex.Male,
            Status = UserStatus.Active,
            BirthDate = new DateOnly(2000, 1, 1),
            AvatarUrl = "avatar.png"
        };

        await sut.SetAsync("cache:user-profile", expected, TimeSpan.FromMinutes(5));
        var result = await sut.GetAsync<User>("cache:user-profile");

        result.Should().NotBeNull();
        result!.Id.Should().Be(expected.Id);
        result.Email.Should().Be(expected.Email);
        result.FirstName.Should().Be(expected.FirstName);
        result.LastName.Should().Be(expected.LastName);
        result.NickName.Should().Be(expected.NickName);
        result.Sex.Should().Be(expected.Sex);
        result.Status.Should().Be(expected.Status);
        result.BirthDate.Should().Be(expected.BirthDate);
        result.AvatarUrl.Should().Be(expected.AvatarUrl);
    }

    [Fact]
    public async Task SetAsync_ShouldStoreAndReturnCategory()
    {
        var sut = CreateSut();
        var expected = new Category
        {
            Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Name = "Books",
            Description = "Fiction books",
            Status = CategoryStatus.Active,
            AdminId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7()
        };

        await sut.SetAsync("cache:category", expected, TimeSpan.FromMinutes(5));
        var result = await sut.GetAsync<Category>("cache:category");

        result.Should().NotBeNull();
        result!.Id.Should().Be(expected.Id);
        result.Name.Should().Be(expected.Name);
        result.Description.Should().Be(expected.Description);
        result.Status.Should().Be(expected.Status);
        result.AdminId.Should().Be(expected.AdminId);
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
    public async Task SetStringAsync_ShouldRemoveOldestKey_WhenPrefixLimitIsExceeded()
    {
        var sut = CreateSut(prefixKeyLimits: new Dictionary<string, int>
        {
            ["Category"] = 2
        });

        await sut.SetStringAsync("Category:first", "1", TimeSpan.FromMinutes(5));
        await sut.SetStringAsync("Category:second", "2", TimeSpan.FromMinutes(5));
        await sut.SetStringAsync("Category:third", "3", TimeSpan.FromMinutes(5));

        var first = await sut.GetStringAsync("Category:first");
        var second = await sut.GetStringAsync("Category:second");
        var third = await sut.GetStringAsync("Category:third");

        first.Should().BeNull();
        second.Should().Be("2");
        third.Should().Be("3");
    }

    [Fact]
    public async Task SetStringAsync_ShouldTrackEachPrefixSeparately_WhenPrefixLimitIsExceeded()
    {
        var sut = CreateSut(prefixKeyLimits: new Dictionary<string, int>
        {
            ["Category"] = 1,
            ["UserProfile"] = 1
        });

        await sut.SetStringAsync("Category:first", "1", TimeSpan.FromMinutes(5));
        await sut.SetStringAsync("UserProfile:first", "2", TimeSpan.FromMinutes(5));
        await sut.SetStringAsync("Category:second", "3", TimeSpan.FromMinutes(5));

        var categoryFirst = await sut.GetStringAsync("Category:first");
        var categorySecond = await sut.GetStringAsync("Category:second");
        var userProfileFirst = await sut.GetStringAsync("UserProfile:first");

        categoryFirst.Should().BeNull();
        categorySecond.Should().Be("3");
        userProfileFirst.Should().Be("2");
    }

    [Fact]
    public async Task SetStringAsync_ShouldIgnorePrefixesWithoutConfiguredLimit()
    {
        var sut = CreateSut(prefixKeyLimits: new Dictionary<string, int>
        {
            ["UserProfile"] = 1
        });

        await sut.SetStringAsync("Category:first", "1", TimeSpan.FromMinutes(5));
        await sut.SetStringAsync("Category:second", "2", TimeSpan.FromMinutes(5));
        await sut.SetStringAsync("Category:third", "3", TimeSpan.FromMinutes(5));

        var first = await sut.GetStringAsync("Category:first");
        var second = await sut.GetStringAsync("Category:second");
        var third = await sut.GetStringAsync("Category:third");

        first.Should().Be("1");
        second.Should().Be("2");
        third.Should().Be("3");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDefaultAndRemoveKey_WhenPayloadCannotBeDeserialized()
    {
        var sut = CreateSut(out var storage);
        storage["test:cache:invalid-profile"] = """{"id":"00000000-0000-0000-0000-000000000001","email":"bad@example.com","firstName":"","lastName":"User","sex":1,"status":1,"birthDate":"2000-01-01"}""";

        var result = await sut.GetAsync<User>("cache:invalid-profile");

        result.Should().BeNull();
        storage.Should().NotContainKey("test:cache:invalid-profile");
    }

    [Fact]
    public async Task GetBulkAsync_ShouldTreatInvalidPayloadAsCacheMiss_AndRemoveBrokenKey()
    {
        var sut = CreateSut(out var storage);
        storage["test:cache:invalid-category"] = """{"id":"00000000-0000-0000-0000-000000000001","name":"Category A","description":"Example","status":1,"adminId":"00000000-0000-0000-0000-000000000000"}""";

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

    private static RedisCacheService CreateSut(Dictionary<string, int> prefixKeyLimits)
    {
        return CreateSut(prefixKeyLimits, out _);
    }

    private static RedisCacheService CreateSut(out Dictionary<string, RedisValue> storage)
    {
        return CreateSut([], out storage);
    }

    private static RedisCacheService CreateSut(
        Dictionary<string, int> prefixKeyLimits,
        out Dictionary<string, RedisValue> storage
    )
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
            .Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) => cacheStorage.ContainsKey(key!));

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
                    InstanceName = "test:",
                    PrefixKeyLimits = prefixKeyLimits
                }
            )
        );
    }

    private sealed record CachePayload(string Name, int Count);
}
