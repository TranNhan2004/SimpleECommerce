using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using SimpleECommerceBackend.Api.Options.OpenTelemetry;

namespace SimpleECommerceBackend.Api.Extensions;

public static class OpenTelemetryExtension
{
    public static IServiceCollection AddAppOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var options = configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>()
            ?? new OpenTelemetryOptions();

        services.Configure<OpenTelemetryOptions>(
            configuration.GetSection(OpenTelemetryOptions.SectionName)
        );

        if (!options.Enabled)
            return services;

        var serviceName = string.IsNullOrWhiteSpace(options.ServiceName)
            ? environment.ApplicationName
            : options.ServiceName.Trim();

        var serviceVersion = string.IsNullOrWhiteSpace(options.ServiceVersion)
            ? typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"
            : options.ServiceVersion.Trim();

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: serviceName,
                serviceVersion: serviceVersion,
                serviceInstanceId: Environment.MachineName
            ))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddSqlClientInstrumentation()
                .AddPrometheusExporter());

        return services;
    }

    public static WebApplication UseAppOpenTelemetry(this WebApplication app)
    {
        var options = app.Configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>()
            ?? new OpenTelemetryOptions();

        if (!options.Enabled)
            return app;

        app.UseOpenTelemetryPrometheusScrapingEndpoint(context =>
            HttpMethods.IsGet(context.Request.Method)
            && string.Equals(
                context.Request.Path,
                options.PrometheusScrapeEndpointPath,
                StringComparison.OrdinalIgnoreCase
            )
        );

        return app;
    }
}
