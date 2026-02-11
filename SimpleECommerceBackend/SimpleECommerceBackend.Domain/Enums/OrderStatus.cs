namespace SimpleECommerceBackend.Domain.Enums;

public enum OrderStatus
{
    PendingPayment = 1,
    ReadyToPickup = 2,
    Shipped = 3,
    AwaitingConfirmation = 4,
    Delivered = 5,
    Cancelled = 6,
    Returned = 7,
    Expired = 99
}