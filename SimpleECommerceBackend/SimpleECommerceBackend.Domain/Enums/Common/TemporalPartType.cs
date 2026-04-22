using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums.Common;

public enum TemporalPartType
{
    [DisplayValue("none")]
    None = 1,

    [DisplayValue("date")]
    Date = 2,

    [DisplayValue("month")]
    Month = 3,

    [DisplayValue("year")]
    Year = 4,
}