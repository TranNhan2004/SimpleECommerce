using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Utils;

public static class RoleUtils
{
    private static readonly Role[] SupportedRoles =
    [
        Role.Customer,
        Role.Seller,
        Role.Admin
    ];

    public static Role DefaultRole => Role.Customer;

    public static IReadOnlyList<Role> GetSupportedRoles()
    {
        return SupportedRoles;
    }

    public static IReadOnlyList<string> GetSupportedRoleNames()
    {
        return SupportedRoles.Select(ToKeycloakRoleName).ToArray();
    }

    public static string GetSupportedRolesDisplay()
    {
        return string.Join(", ", GetSupportedRoleNames());
    }

    public static Role Parse(string? input)
    {
        if (TryParse(input, out var role))
            return role;

        throw new ValidationException(
            RoleErrorCode.InvalidRole,
            $"Invalid role. Must be one of: {GetSupportedRolesDisplay()}",
            new Dictionary<string, object?>
            {
                ["field"] = "Role",
                ["allowedValues"] = GetSupportedRolesDisplay()
            }
        );
    }

    public static bool TryParse(string? input, out Role role)
    {
        role = DefaultRole;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var normalized = input.Trim().ToLowerInvariant();
        return normalized switch
        {
            "customer" => Assign(Role.Customer, out role),
            "seller" => Assign(Role.Seller, out role),
            "admin" => Assign(Role.Admin, out role),
            _ => false
        };
    }

    public static Role ResolvePrimaryRole(IEnumerable<string>? roleNames)
    {
        if (roleNames is null)
            return DefaultRole;

        foreach (var roleName in roleNames)
        {
            if (TryParse(roleName, out var role))
                return role;
        }

        return DefaultRole;
    }

    public static string ToKeycloakRoleName(Role role)
    {
        return role switch
        {
            Role.Customer => "customer",
            Role.Seller => "seller",
            Role.Admin => "admin",
            _ => throw new ValidationException(
                RoleErrorCode.UnsupportedRole,
                $"Unsupported role value: {role}",
                new Dictionary<string, object?>
                {
                    ["field"] = "Role",
                    ["value"] = role
                }
            )
        };
    }

    private static bool Assign(Role parsedRole, out Role role)
    {
        role = parsedRole;
        return true;
    }
}
