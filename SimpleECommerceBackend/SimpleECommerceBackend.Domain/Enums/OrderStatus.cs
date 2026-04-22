using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum OrderStatus
{
    [DisplayValue("pending-payment")]
    PendingPayment = 1,

    [DisplayValue("ready-to-pickup")]
    ReadyToPickup = 2,

    [DisplayValue("shipped")]
    Shipped = 3,

    [DisplayValue("awaiting-confirmation")]
    AwaitingConfirmation = 4,

    [DisplayValue("delivered")]
    Delivered = 5,

    [DisplayValue("cancelled")]
    Cancelled = 6,

    [DisplayValue("returned")]
    Returned = 7,

    [DisplayValue("expired")]
    Expired = 99
}
