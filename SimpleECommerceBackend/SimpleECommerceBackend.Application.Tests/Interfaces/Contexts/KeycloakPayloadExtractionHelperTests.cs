using System.Security.Claims;
using FluentAssertions;
using SimpleECommerceBackend.Application.Interfaces.Contexts;

namespace SimpleECommerceBackend.Application.Tests.Interfaces.Contexts;

public class KeycloakPayloadExtractionHelperTests
{
    [Fact]
    public void FindUserId_ShouldReturnMappedNameIdentifier_WhenSubClaimIsMissing()
    {
        var userId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7().ToString();
        var principal = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, userId));

        var result = KeycloakPayloadExtractionHelper.FindKeycloakSubjectId(principal);

        result.Should().Be(userId);
    }

    [Fact]
    public void FindEmail_ShouldFallbackToPreferredUsername_WhenEmailClaimsAreMissing()
    {
        var principal = CreatePrincipal(new Claim("preferred_username", "seller@example.com"));

        var result = KeycloakPayloadExtractionHelper.FindEmail(principal);

        result.Should().Be("seller@example.com");
    }

    [Fact]
    public void GetRoleNames_ShouldReturnDistinctRoles_FromFlatRealmAndResourceClaims()
    {
        var principal = CreatePrincipal(
            new Claim("roles", "seller"),
            new Claim(ClaimTypes.Role, "admin"),
            new Claim(
                "realm_access",
                "{\"roles\":[\"offline_access\",\"admin\",\"default-roles-simpleecommerce\",\"uma_authorization\"]}"
            ),
            new Claim(
                "resource_access",
                "{\"simple-e-commerce-backend\":{\"roles\":[\"seller\",\"customer\"]},\"account\":{\"roles\":[\"manage-account\"]}}"
            )
        );

        var result = KeycloakPayloadExtractionHelper.GetRoleNames(principal);

        result.Should().BeEquivalentTo(
            "seller",
            "admin",
            "offline_access",
            "default-roles-simpleecommerce",
            "uma_authorization",
            "customer",
            "manage-account"
        );
    }

    [Fact]
    public void GetRoleNames_ShouldIgnoreInvalidJsonPayloads()
    {
        var principal = CreatePrincipal(
            new Claim("realm_access", "\"unexpected-string\""),
            new Claim("resource_access", "{\"account\":\"invalid-shape\"}")
        );

        var result = KeycloakPayloadExtractionHelper.GetRoleNames(principal);

        result.Should().BeEmpty();
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }
}
