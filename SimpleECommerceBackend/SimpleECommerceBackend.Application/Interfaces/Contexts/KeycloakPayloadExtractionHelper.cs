using System.Security.Claims;
using System.Text.Json;

namespace SimpleECommerceBackend.Application.Interfaces.Contexts;

public static class KeycloakPayloadExtractionHelper
{
    public static string? FindUserId(ClaimsPrincipal principal)
    {
        return FindFirstValue(principal.Claims, "sub", ClaimTypes.NameIdentifier);
    }

    public static string? FindEmail(ClaimsPrincipal principal)
    {
        return FindFirstValue(principal.Claims, ClaimTypes.Email, "email", "preferred_username");
    }

    public static IReadOnlyList<string> GetRoleNames(ClaimsPrincipal principal)
    {
        return GetRoleNames(principal.Claims);
    }

    public static IReadOnlyList<string> GetRoleNames(ClaimsIdentity identity)
    {
        return GetRoleNames(identity.Claims);
    }

    private static IReadOnlyList<string> GetRoleNames(IEnumerable<Claim> claims)
    {
        var claimList = claims as IReadOnlyCollection<Claim> ?? [.. claims];

        return [..claimList
            .Where(claim => claim.Type is "roles" or ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Concat(ExtractRealmRoles(FindFirstValue(claimList, "realm_access")))
            .Concat(ExtractResourceRoles(claimList))
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)];
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

    private static IReadOnlyList<string> ExtractRealmRoles(string? realmAccessClaimValue)
    {
        if (string.IsNullOrWhiteSpace(realmAccessClaimValue))
            return [];

        try
        {
            using var document = JsonDocument.Parse(realmAccessClaimValue);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return [];

            if (!document.RootElement.TryGetProperty("roles", out var rolesElement)
                || rolesElement.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            return [..rolesElement.EnumerateArray()
                .Select(roleElement => roleElement.GetString())
                .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
                .Cast<string>()];
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            return [];
        }
    }

    private static IReadOnlyList<string> ExtractResourceRoles(IEnumerable<Claim> claims)
    {
        return [..claims
            .Where(claim => claim.Type == "resource_access")
            .SelectMany(claim => ExtractResourceRoles(claim.Value))
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    private static IReadOnlyList<string> ExtractResourceRoles(string? resourceAccessClaimValue)
    {
        if (string.IsNullOrWhiteSpace(resourceAccessClaimValue))
            return [];

        try
        {
            using var document = JsonDocument.Parse(resourceAccessClaimValue);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return [];

            var roleNames = new List<string>();

            foreach (var resourceProperty in document.RootElement.EnumerateObject())
            {
                if (resourceProperty.Value.ValueKind != JsonValueKind.Object)
                    continue;

                if (!resourceProperty.Value.TryGetProperty("roles", out var rolesElement)
                    || rolesElement.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                roleNames.AddRange(
                    rolesElement.EnumerateArray()
                        .Select(roleElement => roleElement.GetString())
                        .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
                        .Cast<string>()
                );
            }

            return roleNames;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            return [];
        }
    }
}
