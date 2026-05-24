namespace SimpleECommerceBackend.Application.Interfaces.Services.Caching;

public interface ICacheConsumingService
{
    Task InvalidateCacheAsync(
        List<string>? exactKeys = null,
        List<string>? prefixKeys = null
    );
}