using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Infrastructure.Contexts;

namespace SimpleECommerceBackend.Infrastructure.Tests.Contexts;

public class UserContextHolderTests
{
    [Fact]
    public void GetRequired_ShouldReturnCurrentUserContext_WhenClaimsAreValid()
    {
        var userId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(
                    new Claim("sub", userId.ToString()),
                    new Claim("roles", "seller"),
                    new Claim("email", "seller@example.com")
                )
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Role.Should().Be(Role.Seller);
        result.Email.Should().Be("seller@example.com");
    }

    [Fact]
    public void GetRequired_ShouldReturnCurrentUserContext_WhenUserIdClaimIsMapped()
    {
        var userId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim("roles", "seller"),
                    new Claim("email", "seller@example.com")
                )
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Role.Should().Be(Role.Seller);
        result.Email.Should().Be("seller@example.com");
    }

    [Fact]
    public void GetRequired_ShouldReturnCurrentUserContext_WhenRoleIsInRealmAccessClaim()
    {
        var userId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(
                    new Claim("sub", userId.ToString()),
                    new Claim(
                        "realm_access",
                        "{\"roles\":[\"offline_access\",\"admin\",\"default-roles-simpleecommerce\",\"uma_authorization\"]}"
                    ),
                    new Claim("email", "admin@example.com")
                )
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Role.Should().Be(Role.Admin);
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
    public void GetRequired_ShouldThrowForbiddenException_WhenRoleClaimIsMissing()
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal(new Claim("sub", Guid.NewGuid().ToString()))
            }
        };

        var sut = CreateSut(httpContextAccessor);

        var action = () => sut.GetUserContext();

        action.Should().Throw<ForbiddenException>()
            .Which.ErrorCode.Should().Be(CurrentUserErrorCodes.RoleMissing);
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
