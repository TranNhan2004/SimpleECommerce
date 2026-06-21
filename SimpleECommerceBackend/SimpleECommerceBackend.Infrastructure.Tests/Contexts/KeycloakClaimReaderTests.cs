using System.Security.Claims;
using FluentAssertions;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public class KeycloakClaimReaderTests
{
    [Fact]
    public void FindSubjectId_ShouldReturnMappedNameIdentifier_WhenSubClaimIsMissing()
    {
        var userId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7().ToString();
        var principal = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, userId));

        var result = KeycloakClaimReader.FindSubjectId(principal);

        result.Should().Be(userId);
    }

    [Fact]
    public void FindEmail_ShouldFallbackToPreferredUsername_WhenEmailClaimsAreMissing()
    {
        var principal = CreatePrincipal(new Claim("preferred_username", "seller@example.com"));

        var result = KeycloakClaimReader.FindEmail(principal);

        result.Should().Be("seller@example.com");
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }
}
