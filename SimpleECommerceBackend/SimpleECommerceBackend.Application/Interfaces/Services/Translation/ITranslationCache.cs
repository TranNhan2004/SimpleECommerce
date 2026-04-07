namespace SimpleECommerceBackend.Application.Interfaces.Services.Translation;

public interface ITranslationCache
{
    Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync(string key, string value, TimeSpan ttl, CancellationToken cancellationToken = default);
}
