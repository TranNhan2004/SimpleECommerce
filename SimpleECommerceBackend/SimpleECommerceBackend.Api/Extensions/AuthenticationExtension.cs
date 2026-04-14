using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
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
                options.RequireHttpsMetadata = !environment.IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudiences = [keycloakOptions.Resource],
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity identity)
                            return Task.CompletedTask;

                        foreach (var roleName in KeycloakPayloadExtractionHelper.GetRoleNames(identity))
                        {
                            if (!identity.HasClaim(ClaimTypes.Role, roleName))
                                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));

                            if (!identity.HasClaim("roles", roleName))
                                identity.AddClaim(new Claim("roles", roleName));
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
