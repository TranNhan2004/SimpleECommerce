using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Infrastructure.Contexts;

namespace SimpleECommerceBackend.Infrastructure.Tests.Contexts;

public class UserContextHolderTests
{
    [Fact]
    public void GetRequired_ShouldReturnCurrentUserContext_WhenClaimsAreValid()
    {
        var userId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(
                    new Claim("sub", userId.ToString()),
                    new Claim("email", "seller@example.com")
                )
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Email.Should().Be("seller@example.com");
    }

    [Fact]
    public void GetRequired_ShouldReturnCurrentUserContext_WhenUserIdClaimIsMapped()
    {
        var userId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim("email", "seller@example.com")
                )
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Email.Should().Be("seller@example.com");
    }

    [Fact]
    public void GetRequired_ShouldReturnCurrentUserContext_WhenRoleClaimsAreAbsent()
    {
        var userId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(
                    new Claim("sub", userId.ToString()),
                    new Claim("email", "admin@example.com")
                )
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Email.Should().Be("admin@example.com");
    }

    [Fact]
    public void GetRequired_ShouldThrowUnauthorizedException_WhenUserIsNotAuthenticated()
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        var sut = CreateSut(httpContextAccessor);

        var action = () => sut.GetUserContext();

        action.Should().Throw<UnauthorizedException>()
            .Which.ErrorCode.Should().Be(CurrentUserErrorCodes.Unauthenticated);
    }

    [Fact]
    public void GetRequired_ShouldReturnCurrentUserContext_WhenRoleClaimIsMissing()
    {
        var userId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(new Claim("sub", userId.ToString()))
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Email.Should().BeEmpty();
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }

    private static UserContextHolder CreateSut(IHttpContextAccessor httpContextAccessor)
    {
        return new UserContextHolder(httpContextAccessor);
    }
}
