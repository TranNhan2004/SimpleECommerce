using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Domain.Utils;

public static class SexUtils
{
    public static IReadOnlyList<Sex> GetSupportedSexes()
    {
        return EnumUtils.GetSupportedValues<Sex>();
    }

    public static IReadOnlyList<string> GetSupportedSexNames()
    {
        return EnumUtils.GetSupportedNames<Sex>(ToName);
    }

    public static string GetSupportedSexesDisplay()
    {
        return EnumUtils.GetSupportedDisplay<Sex>(ToName);
    }

    public static Sex Parse(string? input)
    {
        return EnumUtils.Parse<Sex>(input, "Sex", SexErrorCode.InvalidSex, ToName);
    }

    public static bool TryParse(string? input, out Sex sex)
    {
        return EnumUtils.TryParse(input, out sex, ToName);
    }

    public static string ToName(Sex sex)
    {
        return EnumUtils.ToName(sex, "Sex", SexErrorCode.UnsupportedSex, value => value.ToString().ToLowerInvariant());
    }
}
