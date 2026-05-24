using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum ProductStatus
{
    [DisplayValue("draft")]
    Draft = 1,

    [DisplayValue("active")]
    Active = 2,

    [DisplayValue("hidden")]
    Hidden = 3
}
