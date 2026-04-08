namespace SimpleECommerceBackend.Infrastructure.Options.RateLimiter;

public class GlobalRateLimiterOptions
{
    public const string SectionName = "GlobalRateLimiterOptions";

    public int TokenLimit { get; init; }
    public int TokensPerPeriod { get; init; }
    public int ReplenishmentPeriodSeconds { get; init; }
    public bool AutoReplenishment { get; init; }
}