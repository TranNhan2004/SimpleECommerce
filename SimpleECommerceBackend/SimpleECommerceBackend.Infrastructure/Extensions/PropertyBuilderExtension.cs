using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleECommerceBackend.Infrastructure.Extensions;

public static class PropertyBuilderExtension
{
    public static PropertyBuilder<TEnum> HasEnumStringConversion<TEnum>(
        this PropertyBuilder<TEnum> propertyBuilder,
        int? maxLength = null
    )
        where TEnum : struct, Enum
    {
        var resolvedMaxLength = maxLength ?? Enum.GetNames<TEnum>().Max(name => name.Length);

        return propertyBuilder
            .HasConversion<string>()
            .HasMaxLength(resolvedMaxLength);
    }
}
