using SimpleECommerceBackend.Api.Authorization;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Extensions;

public static class PolicyExtension
{
    public static IServiceCollection AddKeycloakPolicies(this IServiceCollection services)
    {
        services.AddSingleton<
            Microsoft.AspNetCore.Authorization.IAuthorizationMiddlewareResultHandler,
            AuthorizationErrorResponseHandler
        >();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.RequireCustomerRole, policy =>
                policy.RequireRole(EnumUtils.ToDisplayValue(Role.Customer)));

            options.AddPolicy(AuthorizationPolicies.RequireSellerRole, policy =>
                policy.RequireRole(EnumUtils.ToDisplayValue(Role.Seller)));

            options.AddPolicy(AuthorizationPolicies.RequireAdminRole, policy =>
                policy.RequireRole(EnumUtils.ToDisplayValue(Role.Admin)));
        });

        return services;
    }
}
