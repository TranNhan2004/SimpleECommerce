using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SimpleECommerceBackend.Application.Interfaces.Contexts;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public class CurrentRequestContext : ICurrentRequestContext
{
    private const string AnonymousUserId = "anonymous";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBackgroundJobContextAccessor _backgroundJobContextAccessor;
    private readonly IServerIpAddressResolver _serverIpAddressResolver;

    public CurrentRequestContext(
        IHttpContextAccessor httpContextAccessor,
        IBackgroundJobContextAccessor backgroundJobContextAccessor,
        IServerIpAddressResolver serverIpAddressResolver
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _backgroundJobContextAccessor = backgroundJobContextAccessor;
        _serverIpAddressResolver = serverIpAddressResolver;
    }

    public string UserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
                return ResolveRequestUserId(httpContext);

            return _backgroundJobContextAccessor.JobName ?? AnonymousUserId;
        }
    }

    public string TraceId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null && !string.IsNullOrWhiteSpace(httpContext.TraceIdentifier))
                return httpContext.TraceIdentifier;

            return _backgroundJobContextAccessor.TraceId
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

    private string ResolveRequestUserId(HttpContext httpContext)
    {
        var principal = httpContext.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return AnonymousUserId;

        var keycloakSubjectId = KeycloakPayloadExtractionHelper.FindKeycloakSubjectId(principal);
        return string.IsNullOrWhiteSpace(keycloakSubjectId) ? AnonymousUserId : keycloakSubjectId;
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
