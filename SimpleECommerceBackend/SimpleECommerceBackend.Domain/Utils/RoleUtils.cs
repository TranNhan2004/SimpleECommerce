using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Domain.Utils;

public static class RoleUtils
{
    public static Role DefaultRole => Role.Customer;
    public static IReadOnlyList<Role> GetSupportedRoles()
    {
        return EnumUtils.GetSupportedValues<Role>();
    }

    public static IReadOnlyList<string> GetSupportedRoleNames()
    {
        return EnumUtils.GetSupportedNames<Role>(ToKeycloakRoleName);
    }

    public static string GetSupportedRolesDisplay()
    {
        return EnumUtils.GetSupportedDisplay<Role>(ToKeycloakRoleName);
    }

    public static Role Parse(string? input)
    {
        return EnumUtils.Parse<Role>(input, "Role", RoleErrorCode.InvalidRole, ToKeycloakRoleName);
    }

    public static bool TryParse(string? input, out Role role)
    {
        return EnumUtils.TryParse(input, out role, ToKeycloakRoleName);
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
        return EnumUtils.ToName(role, "Role", RoleErrorCode.UnsupportedRole, value => value.ToString().ToLowerInvariant());
    }
}
