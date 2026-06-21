namespace SimpleECommerceBackend.Api.Options.OpenTelemetry;

public class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public bool Enabled { get; init; } = true;
    public string ServiceName { get; init; } = "simple-ecommerce-api";
    public string? ServiceVersion { get; init; }
    public string PrometheusScrapeEndpointPath { get; init; } = "/metrics";
}
