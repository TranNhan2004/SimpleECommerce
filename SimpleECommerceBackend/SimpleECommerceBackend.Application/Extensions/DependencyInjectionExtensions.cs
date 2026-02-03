using Microsoft.Extensions.DependencyInjection;

namespace SimpleECommerceBackend.Application.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}