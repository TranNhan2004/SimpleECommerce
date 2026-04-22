using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums.Common;

public enum FilterOperator
{
    [DisplayValue("==")]
    Equal = 1,

    [DisplayValue(">")]
    GreaterThan = 2,

    [DisplayValue("<")]
    LessThan = 3,

    [DisplayValue(">=")]
    GreaterThanOrEqual = 4,

    [DisplayValue("<=")]
    LessThanOrEqual = 5,

    [DisplayValue("contains")]
    Contains = 6,

    [DisplayValue("starts with")]
    StartsWith = 7,

    [DisplayValue("ends with")]
    EndsWith = 8,

    [DisplayValue("is null")]
    IsNull = 9,
}
