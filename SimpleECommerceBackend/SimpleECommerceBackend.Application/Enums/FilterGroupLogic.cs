using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Application.Enums;

public enum FilterGroupLogic
{
    [DisplayValue("not")]
    Not = 1,

    [DisplayValue("and")]
    And = 2,

    [DisplayValue("or")]
    Or = 3
}