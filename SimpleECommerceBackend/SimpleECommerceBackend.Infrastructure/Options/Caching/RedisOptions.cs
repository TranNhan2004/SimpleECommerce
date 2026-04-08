namespace SimpleECommerceBackend.Infrastructure.Options.Caching;

public class RedisOptions
{
    public const string SectionName = "RedisOptions";

    public string ConnectionString { get; init; } = null!;
    public string InstanceName { get; init; } = null!;
}