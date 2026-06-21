using System.Security.Claims;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public static class KeycloakClaimReader
{
    public static string? FindSubjectId(ClaimsPrincipal principal)
    {
        return FindFirstValue(principal.Claims, "sub", ClaimTypes.NameIdentifier);
    }

    public static string? FindEmail(ClaimsPrincipal principal)
    {
        return FindFirstValue(principal.Claims, ClaimTypes.Email, "email", "preferred_username");
    }

    private static string? FindFirstValue(IEnumerable<Claim> claims, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var claimValue = claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
            if (!string.IsNullOrWhiteSpace(claimValue))
                return claimValue;
        }

        return null;
    }
}
