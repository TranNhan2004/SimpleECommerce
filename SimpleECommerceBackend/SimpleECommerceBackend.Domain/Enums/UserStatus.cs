using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum UserStatus
{
    [DisplayValue("active")]
    Active = 1,

    [DisplayValue("archived")]
    Archived = 99
}
