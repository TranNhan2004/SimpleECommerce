namespace SimpleECommerceBackend.Infrastructure.Options.RateLimiter;

public class IpRateLimiterOptions
{
    public const string SectionName = "IpRateLimiterOptions";

    public int TokenLimit { get; init; }
    public int TokensPerPeriod { get; init; }
    public int ReplenishmentPeriodSeconds { get; init; }
    public bool AutoReplenishment { get; init; }
}