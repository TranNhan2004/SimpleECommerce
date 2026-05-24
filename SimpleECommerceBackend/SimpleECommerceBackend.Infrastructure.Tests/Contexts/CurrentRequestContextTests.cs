using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Infrastructure.Contexts;

namespace SimpleECommerceBackend.Infrastructure.Tests.Contexts;

public class CurrentRequestContextTests
{
    [Fact]
    public void ShouldResolveUserTraceAndForwardedIp_FromHttpContext()
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                TraceIdentifier = "request-trace-id",
                User = CreatePrincipal(new Claim("sub", "user-123"))
            }
        };
        httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"] = "198.51.100.10, 10.0.0.1";

        var sut = CreateSut(httpContextAccessor);

        sut.UserId.Should().Be("user-123");
        sut.TraceId.Should().Be("request-trace-id");
        sut.IpAddress.Should().Be("198.51.100.10");
    }

    [Fact]
    public void ShouldFallbackToAnonymous_WhenRequestUserIsUnauthenticated()
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                TraceIdentifier = "request-trace-id"
            }
        };
        httpContextAccessor.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.0.2.44");

        var sut = CreateSut(httpContextAccessor);

        sut.UserId.Should().Be("anonymous");
        sut.TraceId.Should().Be("request-trace-id");
        sut.IpAddress.Should().Be("192.0.2.44");
    }

    [Fact]
    public void ShouldResolveBackgroundJobContext_WhenHttpContextIsUnavailable()
    {
        var httpContextAccessor = new HttpContextAccessor();
        var backgroundJobContextAccessor = new BackgroundJobContextAccessor();

        using (backgroundJobContextAccessor.BeginScope("HardDeleteBackgroundWorker", "background-trace-id"))
        {
            var sut = new CurrentRequestContext(
                httpContextAccessor,
                backgroundJobContextAccessor,
                new StubServerIpAddressResolver("10.10.10.10")
            );

            sut.UserId.Should().Be("HardDeleteBackgroundWorker");
            sut.TraceId.Should().Be("background-trace-id");
            sut.IpAddress.Should().Be("10.10.10.10");
        }
    }

    private static CurrentRequestContext CreateSut(IHttpContextAccessor httpContextAccessor)
    {
        return new CurrentRequestContext(
            httpContextAccessor,
            new BackgroundJobContextAccessor(),
            new StubServerIpAddressResolver("127.0.0.1")
        );
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }

    private sealed class StubServerIpAddressResolver(string ipAddress) : IServerIpAddressResolver
    {
        public string GetIpAddress()
        {
            return ipAddress;
        }
    }
}
