namespace SimpleECommerceBackend.Infrastructure.Services.Caching;

public sealed class PrefixKeyCacheExtractor
{
    public string ExtractPrefix(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var normalizedKey = key.Trim();
        var separatorIndex = normalizedKey.IndexOf(':');

        return separatorIndex < 0
            ? normalizedKey
            : normalizedKey[..separatorIndex];
    }
}
