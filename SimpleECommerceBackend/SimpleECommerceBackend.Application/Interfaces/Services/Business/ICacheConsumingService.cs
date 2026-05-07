namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface ICacheConsumingService
{
    Task InvalidateCacheAsync(
        List<string>? exactKeys = null,
        List<string>? prefixKeys = null
    );
}