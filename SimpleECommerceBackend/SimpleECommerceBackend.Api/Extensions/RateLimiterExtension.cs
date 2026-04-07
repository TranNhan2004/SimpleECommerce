using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Infrastructure.Services.RateLimiter;

namespace SimpleECommerceBackend.Api.Extensions;

public static class IpRateLimitExtension
{
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GlobalRateLimiterOptions>(configuration.GetSection("GlobalRateLimiterOptions"));
        services.Configure<IpRateLimiterOptions>(configuration.GetSection("IpRateLimiterOptions"));
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddRateLimiter(options =>
        {
            // Global limiter
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var globalOptions = context.RequestServices.GetRequiredService<IOptions<GlobalRateLimiterOptions>>().Value;

                return RateLimitPartition.GetTokenBucketLimiter("global", _ =>
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = globalOptions.TokenLimit,
                        TokensPerPeriod = globalOptions.TokensPerPeriod,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(globalOptions.ReplenishmentPeriodSeconds),
                        AutoReplenishment = globalOptions.AutoReplenishment
                    });
            });

            // Per-IP-Route limiter
            options.AddPolicy("ip-route", context =>
            {
                var ipOptions = context.RequestServices.GetRequiredService<IOptions<IpRateLimiterOptions>>().Value;
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var route = context.Request.RouteValues["controller"] + ":" +
                            context.Request.RouteValues["action"];
                var key = $"{ip}:{route}";

                return RateLimitPartition.GetTokenBucketLimiter(key, _ =>
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = ipOptions.TokenLimit,
                        TokensPerPeriod = ipOptions.TokensPerPeriod,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(ipOptions.ReplenishmentPeriodSeconds),
                        AutoReplenishment = ipOptions.AutoReplenishment
                    });
            });

            options.OnRejected = async (context, token) =>
            {
                var httpContext = context.HttpContext;

                var ip = httpContext.Connection.RemoteIpAddress?.ToString();

                var logger = httpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("RateLimiter");

                logger.LogWarning(
                    "Rate limit exceeded. IP: {IP}, Path: {Path}, Method: {Method}",
                    ip,
                    httpContext.Request.Path,
                    httpContext.Request.Method
                );

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await httpContext.Response.WriteAsync("Too many requests", token);
            };
        });

        return services;
    }
}