using Microsoft.Extensions.Configuration;

namespace SimpleECommerceBackend.Infrastructure.Extensions;

public static class ConfigurationExtension
{
    public static TOptions GetRequiredOptions<TOptions>(
        this IConfiguration configuration,
        string sectionName)
        where TOptions : class, new()
    {
        var options = configuration.GetSection(sectionName).Get<TOptions>();

        return options ?? throw new InvalidOperationException(
            $"Configuration section '{sectionName}' is missing or invalid."
        );
    }
}