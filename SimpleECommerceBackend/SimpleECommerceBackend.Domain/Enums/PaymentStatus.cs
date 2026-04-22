using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum PaymentStatus
{
    [DisplayValue("pending")]
    Pending = 1,

    [DisplayValue("completed")]
    Completed = 2,

    [DisplayValue("failed")]
    Failed = 3,

    [DisplayValue("refunded")]
    Refunded = 4
}
