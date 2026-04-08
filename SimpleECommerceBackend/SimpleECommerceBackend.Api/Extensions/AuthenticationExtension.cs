using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimpleECommerceBackend.Infrastructure.Extensions;
using SimpleECommerceBackend.Infrastructure.Options.Authentication;

namespace SimpleECommerceBackend.Api.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        var keycloakOptions = configuration.GetRequiredOptions<KeycloakOptions>(KeycloakOptions.SectionName);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{keycloakOptions.AuthServerUrl}/realms/{keycloakOptions.Realm}/";
                options.Audience = keycloakOptions.Resource;
                options.RequireHttpsMetadata = !environment.IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = keycloakOptions.VerifyTokenAudience,
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles"
                };
            });

        return services;
    }
}