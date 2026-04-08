using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Extensions;

public static class PolicyExtension
{
    public static IServiceCollection AddKeycloakPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(RoleUtils.RequireCustomerRole, policy =>
                policy.RequireRole(RoleUtils.ToKeycloakRoleName(Role.Customer)));

            options.AddPolicy(RoleUtils.RequireSellerRole, policy =>
                policy.RequireRole(RoleUtils.ToKeycloakRoleName(Role.Seller)));

            options.AddPolicy(RoleUtils.RequireAdminRole, policy =>
                policy.RequireRole(RoleUtils.ToKeycloakRoleName(Role.Admin)));
        });

        return services;
    }
}