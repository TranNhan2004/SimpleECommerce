using System.Text;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Utils;

public static class EnumUtils
{
    public static IReadOnlyList<TEnum> GetSupportedValues<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>();
    }

    public static IReadOnlyList<string> GetSupportedNames<TEnum>(Func<TEnum, string>? nameSelector = null)
        where TEnum : struct, Enum
    {
        return [.. GetSupportedValues<TEnum>().Select(value => FormatName(value, nameSelector))];
    }

    public static string GetSupportedDisplay<TEnum>(Func<TEnum, string>? nameSelector = null)
        where TEnum : struct, Enum
    {
        return string.Join(", ", GetSupportedNames(nameSelector));
    }

    public static TEnum Parse<TEnum>(
        string? input,
        string fieldName,
        string invalidErrorCode,
        Func<TEnum, string>? nameSelector = null)
        where TEnum : struct, Enum
    {
        if (TryParse(input, out TEnum value, nameSelector))
            return value;

        var allowedValues = GetSupportedDisplay(nameSelector);
        throw new ValidationException(
            invalidErrorCode,
            $"Invalid {fieldName}. Must be one of: {allowedValues}",
            new Dictionary<string, object?>
            {
                ["field"] = fieldName,
                ["allowedValues"] = allowedValues
            }
        );
    }

    public static bool TryParse<TEnum>(string? input, out TEnum value, Func<TEnum, string>? nameSelector = null)
        where TEnum : struct, Enum
    {
        value = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var normalizedInput = Normalize(input);
        if (normalizedInput.Length == 0)
            return false;

        foreach (var candidate in GetSupportedValues<TEnum>())
        {
            if (!IsMatch(candidate, normalizedInput, nameSelector))
                continue;

            value = candidate;
            return true;
        }

        return false;
    }

    public static string ToName<TEnum>(
        TEnum value,
        string fieldName,
        string unsupportedErrorCode,
        Func<TEnum, string>? nameSelector = null)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ValidationException(
                unsupportedErrorCode,
                $"Unsupported {fieldName} value: {value}",
                new Dictionary<string, object?>
                {
                    ["field"] = fieldName,
                    ["value"] = value
                }
            );
        }

        return FormatName(value, nameSelector);
    }

    private static bool IsMatch<TEnum>(TEnum value, string normalizedInput, Func<TEnum, string>? nameSelector)
        where TEnum : struct, Enum
    {
        if (Normalize(value.ToString()).Equals(normalizedInput, StringComparison.Ordinal))
            return true;

        if (nameSelector is null)
            return false;

        return Normalize(nameSelector(value)).Equals(normalizedInput, StringComparison.Ordinal);
    }

    private static string FormatName<TEnum>(TEnum value, Func<TEnum, string>? nameSelector)
        where TEnum : struct, Enum
    {
        return nameSelector?.Invoke(value) ?? value.ToString();
    }

    private static string Normalize(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var character in value.Trim())
        {
            if (char.IsLetterOrDigit(character))
                builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
