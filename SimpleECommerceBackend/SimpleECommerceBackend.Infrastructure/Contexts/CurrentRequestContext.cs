using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Constants.Uam;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public class CurrentRequestContext : ICurrentRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IBackgroundJobContext _backgroundJobContext;
    private readonly IServerIpAddressContext _serverIpAddressResolver;

    public CurrentRequestContext(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserContext currentUserContext,
        IBackgroundJobContext backgroundJobContext,
        IServerIpAddressContext serverIpAddressResolver
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUserContext = currentUserContext;
        _backgroundJobContext = backgroundJobContext;
        _serverIpAddressResolver = serverIpAddressResolver;
    }

    public Guid ActorId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
                return ResolveRequestActorId(httpContext);

            return WellKnownUserIds.System;
        }
    }

    public string TraceId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null && !string.IsNullOrWhiteSpace(httpContext.TraceIdentifier))
                return httpContext.TraceIdentifier;

            return _backgroundJobContext.TraceId
                ?? Activity.Current?.Id
                ?? Guid.NewGuid().ToString("N");
        }
    }

    public string IpAddress
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
                return ResolveRequestIpAddress(httpContext) ?? _serverIpAddressResolver.GetIpAddress();

            return _serverIpAddressResolver.GetIpAddress();
        }
    }

    private Guid ResolveRequestActorId(HttpContext httpContext)
    {
        var principal = httpContext.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return WellKnownUserIds.Anonymous;

        return _currentUserContext.Id;
    }

    private string? ResolveRequestIpAddress(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwardedForValues))
        {
            var forwardedFor = forwardedForValues.ToString();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .FirstOrDefault();
            }
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}
