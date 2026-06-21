using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SimpleECommerceBackend.Infrastructure.Extensions;
using SimpleECommerceBackend.Infrastructure.Options.Authentication;
using SimpleECommerceBackend.Api.Authentication;

namespace SimpleECommerceBackend.Api.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        configuration.GetRequiredOptions<AuthOptions>(AuthOptions.SectionName);
        configuration.GetRequiredOptions<KeycloakBffOptions>(KeycloakBffOptions.SectionName);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AppAuthenticationDefaults.SessionScheme;
                options.DefaultChallengeScheme = AppAuthenticationDefaults.SessionScheme;
                options.DefaultForbidScheme = AppAuthenticationDefaults.SessionScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
                AppAuthenticationDefaults.SessionScheme,
                _ => { }
            );

        return services;
    }
}
