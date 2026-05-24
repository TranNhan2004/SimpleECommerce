using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum UserStatus
{
    [DisplayValue("pending")]
    Pending = 0,

    [DisplayValue("active")]
    Active = 1,

    [DisplayValue("archived")]
    Archived = 99
}
