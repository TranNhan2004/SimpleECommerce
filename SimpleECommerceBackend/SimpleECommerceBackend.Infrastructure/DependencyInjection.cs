using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Translation;
using SimpleECommerceBackend.Application.Interfaces.Services.Address;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Infrastructure.Contexts;
using SimpleECommerceBackend.Infrastructure.Extensions;
using SimpleECommerceBackend.Infrastructure.Options.Authentication;
using SimpleECommerceBackend.Infrastructure.Options.Caching;
using SimpleECommerceBackend.Infrastructure.Options.Email;
using SimpleECommerceBackend.Infrastructure.Options.RateLimiter;
using SimpleECommerceBackend.Infrastructure.Options.Translation;
using SimpleECommerceBackend.Infrastructure.Persistence;
using SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;
using SimpleECommerceBackend.Infrastructure.Repositories;
using SimpleECommerceBackend.Infrastructure.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Repositories.Translation;
using SimpleECommerceBackend.Infrastructure.Services.Address;
using SimpleECommerceBackend.Infrastructure.Services.Caching;
using SimpleECommerceBackend.Infrastructure.Services.Email;
using SimpleECommerceBackend.Infrastructure.Services.Translation;

namespace SimpleECommerceBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        // Authentication
        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.SectionName));

        // Caching
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));

        // Email
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        // Rate Limiter
        services.Configure<GlobalRateLimiterOptions>(configuration.GetSection(GlobalRateLimiterOptions.SectionName));
        services.Configure<IpRateLimiterOptions>(configuration.GetSection(IpRateLimiterOptions.SectionName));

        // Translation
        services.Configure<TranslationOptions>(configuration.GetSection(TranslationOptions.SectionName));
        services.Configure<OpenAITranslationOptions>(configuration.GetSection(OpenAITranslationOptions.SectionName));
        services.Configure<GoogleAITranslationOptions>(configuration.GetSection(GoogleAITranslationOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Email Services
        services.AddScoped<IEmailBodyProvider, EmailBodyProvider>();
        services.AddSingleton<BackgroundEmailQueue>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddHostedService<EmailBackgroundWorker>();

        // Address Services
        services.AddSingleton<IAddressService, VnAddressService>();

        // Request User Context
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextHolder, UserContextHolder>();

        // Cache Services
        var redisOptions = configuration.GetRequiredOptions<RedisOptions>(RedisOptions.SectionName);
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisOptions.ConnectionString;
            options.InstanceName = redisOptions.InstanceName;
        });
        services.AddScoped<ICacheService, DistributedCacheService>();

        // Translation Services
        services.AddSingleton<IStaticTextLocalizer, JsonStaticTextLocalizer>();
        services.AddScoped<IDynamicTranslationService, DynamicTranslationService>();
        services.AddScoped<ITranslationEntryRepository, TranslationEntryRepository>();
        services.AddScoped<ITranslationProvider, GoogleAITranslationProvider>();

        // services.AddSingleton<NullTranslationProvider>();
        // services.AddScoped<OpenAITranslationProvider>();
        // services.AddScoped<GoogleAITranslationProvider>();
        // services.AddSingleton<ITranslationProvider>(sp => sp.GetRequiredService<NullTranslationProvider>());
        // services.AddScoped<ITranslationProvider>(sp => sp.GetRequiredService<OpenAITranslationProvider>());
        // services.AddScoped<ITranslationProvider>(sp => sp.GetRequiredService<GoogleAITranslationProvider>());

        // Database
        services.AddScoped<AuditSaveChangesInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Business Repositories
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerShippingAddressRepository, CustomerShippingAddressRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISellerShopRepository, SellerShopRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        return services;
    }
}