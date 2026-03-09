using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Extensions;

public static class PolicyExtension
{
    public const string RequireAdminRole = "__SimpleECommerce_RequireAdminRole__";
    public const string RequireSellerRole = "__SimpleECommerce_RequireSellerRole__";
    public const string RequireCustomerRole = "__SimpleECommerce_RequireCustomerRole__";


    public static IServiceCollection AddKeycloakPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(RequireCustomerRole, policy =>
                policy.RequireRole(RoleUtils.ToKeycloakRoleName(Role.Customer)));

            options.AddPolicy(RequireSellerRole, policy =>
                policy.RequireRole(RoleUtils.ToKeycloakRoleName(Role.Seller)));

            options.AddPolicy(RequireAdminRole, policy =>
                policy.RequireRole(RoleUtils.ToKeycloakRoleName(Role.Admin)));
        });

        return services;
    }
}