namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface ICacheConsumingService
{
    Task InvalidateCacheByIdAsync(Guid id);
    Task InvalidateCacheByKeyAsync(string key);
}