using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Interfaces.Time;
using SimpleECommerceBackend.Infrastructure.Persistence.AppDbContext;
using SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;
using SimpleECommerceBackend.Infrastructure.Repositories.Auth;
using SimpleECommerceBackend.Infrastructure.Repositories.Business;
using SystemClock = SimpleECommerceBackend.Infrastructure.Time.SystemClock;

namespace SimpleECommerceBackend.Infrastructure.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Time
        services.AddSingleton<IClock, SystemClock>();

        // Db
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // Unit of work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Repositories
        // Auth
        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        // Business
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        return services;
    }
}