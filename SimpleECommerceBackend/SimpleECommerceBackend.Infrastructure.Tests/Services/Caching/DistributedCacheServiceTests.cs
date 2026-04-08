using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SimpleECommerceBackend.Infrastructure.Services.Caching;

namespace SimpleECommerceBackend.Infrastructure.Tests.Services.Caching;

public class DistributedCacheServiceTests
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

    private static DistributedCacheService CreateSut()
    {
        IDistributedCache cache =
            new MemoryDistributedCache(
                Microsoft.Extensions.Options.Options.Create(new MemoryDistributedCacheOptions()));
        return new DistributedCacheService(cache);
    }

    private sealed record CachePayload(string Name, int Count);
}