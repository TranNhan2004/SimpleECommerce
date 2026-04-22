using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums.Common;

public enum FilterGroupLogic
{
    [DisplayValue("not")]
    Not = 1,

    [DisplayValue("and")]
    And = 2,

    [DisplayValue("or")]
    Or = 3
}