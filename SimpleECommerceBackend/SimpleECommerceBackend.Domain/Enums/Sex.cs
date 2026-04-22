using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum Sex
{
    [DisplayValue("male")]
    Male = 1,

    [DisplayValue("female")]
    Female = 2,

    [DisplayValue("other")]
    Other = 3
}
