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

        var sut = new UserContextHolder(httpContextAccessor);

        var result = sut.GetUserContext();

        result.Id.Should().Be(userId);
        result.Role.Should().Be(Role.Seller);
        result.Email.Should().Be("seller@example.com");
    }

    [Fact]
    public void GetRequired_ShouldThrowUnauthorizedException_WhenUserIsNotAuthenticated()
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        var sut = new UserContextHolder(httpContextAccessor);

        var action = () => sut.GetUserContext();

        action.Should().Throw<UnauthorizedException>()
            .Which.ErrorCode.Should().Be(CurrentUserErrorCode.Unauthenticated);
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

        var sut = new UserContextHolder(httpContextAccessor);

        var action = () => sut.GetUserContext();

        action.Should().Throw<ForbiddenException>()
            .Which.ErrorCode.Should().Be(CurrentUserErrorCode.RoleMissing);
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }
}
