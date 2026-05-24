using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum CredentialStatus
{
    [DisplayValue("active")]
    Active = 1,

    [DisplayValue("inactive")]
    Inactive = 2,

    [DisplayValue("archived")]
    Archived = 99
}
