using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Domain.Utils;

public static class UserStatusUtils
{
    public static IReadOnlyList<UserStatus> GetSupportedUserStatuses()
    {
        return EnumUtils.GetSupportedValues<UserStatus>();
    }

    public static IReadOnlyList<string> GetSupportedUserStatusNames()
    {
        return EnumUtils.GetSupportedNames<UserStatus>(ToName);
    }

    public static string GetSupportedUserStatusesDisplay()
    {
        return EnumUtils.GetSupportedDisplay<UserStatus>(ToName);
    }

    public static UserStatus Parse(string? input)
    {
        return EnumUtils.Parse<UserStatus>(input, "UserStatus", UserStatusErrorCode.InvalidUserStatus, ToName);
    }

    public static bool TryParse(string? input, out UserStatus status)
    {
        return EnumUtils.TryParse(input, out status, ToName);
    }

    public static string ToName(UserStatus status)
    {
        return EnumUtils.ToName(
            status,
            "UserStatus",
            UserStatusErrorCode.UnsupportedUserStatus,
            value => value.ToString().ToLowerInvariant());
    }
}
