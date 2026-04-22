using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums.Common;

public enum FieldType
{
    [DisplayValue("int")]
    Int,

    [DisplayValue("long")]
    Long,

    [DisplayValue("decimal")]
    Decimal,

    [DisplayValue("float")]
    Float,

    [DisplayValue("double")]
    Double,

    [DisplayValue("datetime")]
    DateTimeOffset,

    [DisplayValue("date")]
    DateOnly,

    [DisplayValue("string")]
    String,

    [DisplayValue("bool")]
    Bool
}