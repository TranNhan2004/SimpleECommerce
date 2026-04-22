using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum Role
{
    [DisplayValue("admin")]
    Admin = 1,

    [DisplayValue("seller")]
    Seller = 2,

    [DisplayValue("customer")]
    Customer = 3
}
