using System.Reflection;
using SimpleECommerceBackend.Domain.Attributes;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Utils;

public static class EnumUtils
{
    public static string ToDisplayValue<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ValidationException(
                EnumConversionErrorCodes.UnsupportedValue,
                $"Unsupported {typeof(TEnum).Name} value: {value}."
            );
        }

        var member = typeof(TEnum).GetMember(value.ToString()).SingleOrDefault();
        var displayValueAttribute = member?.GetCustomAttribute<DisplayValueAttribute>();

        if (displayValueAttribute is null || string.IsNullOrWhiteSpace(displayValueAttribute.Value))
        {
            throw new ValidationException(
                EnumConversionErrorCodes.UnsupportedValue,
                $"Missing display value for {typeof(TEnum).Name}.{value}."
            );
        }

        return displayValueAttribute.Value;
    }

    public static TEnum FromDisplayValue<TEnum>(string displayValue)
        where TEnum : struct, Enum
    {
        var trimmedDisplayValue = displayValue?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedDisplayValue))
        {
            throw new ValidationException(
                EnumConversionErrorCodes.InvalidDisplayValue,
                $"Invalid {typeof(TEnum).Name} display value."
            );
        }

        var supportedDisplayValues = new List<string>();

        foreach (var value in Enum.GetValues<TEnum>())
        {
            var candidateDisplayValue = ToDisplayValue(value);
            supportedDisplayValues.Add(candidateDisplayValue);

            if (string.Equals(candidateDisplayValue, trimmedDisplayValue, StringComparison.OrdinalIgnoreCase))
                return value;
        }

        throw new ValidationException(
            EnumConversionErrorCodes.InvalidDisplayValue,
            $"Invalid {typeof(TEnum).Name} display value '{trimmedDisplayValue}'. Supported values: {string.Join(", ", supportedDisplayValues)}."
        );
    }
}
