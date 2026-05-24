using SimpleECommerceBackend.Api.Authorization;

namespace SimpleECommerceBackend.Api.Extensions;

public static class PolicyExtension
{
    public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<
            Microsoft.AspNetCore.Authorization.IAuthorizationMiddlewareResultHandler,
            AuthorizationErrorResponseHandler
        >();

        services.AddAuthorization();

        return services;
    }
}
