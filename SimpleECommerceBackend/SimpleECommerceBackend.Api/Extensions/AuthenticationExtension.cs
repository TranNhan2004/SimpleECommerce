using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        var keycloakAuthServerUrl = configuration["Keycloak:AuthServerUrl"]?.TrimEnd('/') ?? string.Empty;
        var keycloakRealm = configuration["Keycloak:Realm"];
        var keycloakAudience = configuration["Keycloak:Resource"];
        var keycloakIssuer = $"{keycloakAuthServerUrl}/realms/{keycloakRealm}/";
        var keycloakVerifyTokenAudienceValue = configuration["Keycloak:VerifyTokenAudience"];
        var keycloakVerifyTokenAudience = !bool.TryParse(keycloakVerifyTokenAudienceValue, out var parsedVerifyTokenAudience)
            || parsedVerifyTokenAudience;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakIssuer;
                options.Audience = keycloakAudience;
                options.RequireHttpsMetadata = !environment.IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = keycloakVerifyTokenAudience,
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles"
                };
            });

        return services;
    }
}