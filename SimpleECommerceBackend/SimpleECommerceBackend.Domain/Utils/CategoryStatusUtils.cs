using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Domain.Utils;

public static class CategoryStatusUtils
{
    public static IReadOnlyList<CategoryStatus> CategoryStatuses()
    {
        return EnumUtils.GetSupportedValues<CategoryStatus>();
    }

    public static IReadOnlyList<string> CategoryStatusNames()
    {
        return EnumUtils.GetSupportedNames<CategoryStatus>(ToName);
    }

    public static string CategoryStatusesDisplay()
    {
        return EnumUtils.GetSupportedDisplay<CategoryStatus>(ToName);
    }

    public static CategoryStatus Parse(string? input)
    {
        return EnumUtils.Parse<CategoryStatus>(input, "CategoryStatus", CategoryStatusErrorCode.InvalidCategoryStatus, ToName);
    }

    public static bool TryParse(string? input, out CategoryStatus categoryStatus)
    {
        return EnumUtils.TryParse(input, out categoryStatus, ToName);
    }

    public static string ToName(CategoryStatus categoryStatus)
    {
        return EnumUtils.ToName(categoryStatus, "CategoryStatus", CategoryStatusErrorCode.UnsupportCategoryStatus, value => value.ToString().ToLowerInvariant());
    }
}