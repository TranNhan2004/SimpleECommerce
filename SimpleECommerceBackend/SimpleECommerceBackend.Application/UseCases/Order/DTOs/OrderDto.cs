namespace SimpleECommerceBackend.Application.UseCases.Order.DTOs;

public record OrderDto(
    Guid Id,
    string Code,
    string Status,
    decimal TotalAmount,
    string Currency,
    DateTimeOffset CreatedAt,
    List<OrderItemDto> Items,
    OrderShippingAddressDto? ShippingAddress,
    string? PaymentStatus,
    string? PaymentMethod
);
