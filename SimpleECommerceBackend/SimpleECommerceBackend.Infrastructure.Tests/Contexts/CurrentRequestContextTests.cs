using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Constants.Uam;
using SimpleECommerceBackend.Infrastructure.Contexts;

namespace SimpleECommerceBackend.Infrastructure.Tests.Contexts;

public class CurrentRequestContextTests
{
    [Fact]
    public void ShouldResolveUserTraceAndForwardedIp_FromHttpContext()
    {
        var expectedActorId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                TraceIdentifier = "request-trace-id",
                User = CreatePrincipal(new Claim("sub", Guid.NewGuid().ToString()))
            }
        };
        httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"] = "198.51.100.10, 10.0.0.1";

        var sut = CreateSut(httpContextAccessor, new StubCurrentUserContext(expectedActorId));

        sut.ActorId.Should().Be(expectedActorId);
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

        var sut = CreateSut(httpContextAccessor, new StubCurrentUserContext(Guid.NewGuid()));

        sut.ActorId.Should().Be(WellKnownUserIds.Anonymous);
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
                new StubCurrentUserContext(Guid.NewGuid()),
                backgroundJobContextAccessor,
                new StubServerIpAddressResolver("10.10.10.10")
            );

            sut.ActorId.Should().Be(WellKnownUserIds.System);
            sut.TraceId.Should().Be("background-trace-id");
            sut.IpAddress.Should().Be("10.10.10.10");
        }
    }

    private static CurrentRequestContext CreateSut(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserContext currentUserContext
    )
    {
        return new CurrentRequestContext(
            httpContextAccessor,
            currentUserContext,
            new BackgroundJobContextAccessor(),
            new StubServerIpAddressResolver("127.0.0.1")
        );
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }

    private sealed class StubCurrentUserContext(Guid id) : ICurrentUserContext
    {
        public Guid Id { get; } = id;
        public Guid KeycloakSubjectId { get; } = Guid.NewGuid();
        public string Email { get; } = "test@example.com";
    }

    private sealed class StubServerIpAddressResolver(string ipAddress) : IServerIpAddressContext
    {
        public string GetIpAddress()
        {
            return ipAddress;
        }
    }
}
