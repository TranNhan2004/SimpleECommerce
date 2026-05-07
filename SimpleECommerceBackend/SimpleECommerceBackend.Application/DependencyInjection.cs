using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Services;
using SimpleECommerceBackend.Application.UseCases;

namespace SimpleECommerceBackend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddServices();

        // Use Cases
        services.AddUseCaseHandlersAndDispatcher();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }

    private static IServiceCollection AddUseCaseHandlersAndDispatcher(
        this IServiceCollection services,
        params Type[] excludedImplementationTypes
    )
    {
        var excludedSet = excludedImplementationTypes.ToHashSet();

        var assemblies = new[]
        {
            typeof(IUseCaseHandler<>).Assembly,
            typeof(IUseCaseHandler<,>).Assembly
        };

        var allTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                !excludedSet.Contains(t));

        var count = 0;
        foreach (var implementationType in allTypes)
        {
            var serviceTypes = implementationType
                .GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    (
                        i.GetGenericTypeDefinition() == typeof(IUseCaseHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(IUseCaseHandler<,>)
                    ));

            foreach (var serviceType in serviceTypes)
                services.AddScoped(serviceType, implementationType);

            count += serviceTypes?.Count() ?? 0;
        }

        Console.WriteLine($"Registering Use Case Handlers: {count} found.");

        services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();

        return services;
    }
}