namespace SimpleECommerceBackend.Infrastructure.Options.Maintenance;

public sealed class HardDeleteOptions
{
    public const string SectionName = "HardDeleteOptions";

    public int DaysUntilHardDelete { get; init; }
}
