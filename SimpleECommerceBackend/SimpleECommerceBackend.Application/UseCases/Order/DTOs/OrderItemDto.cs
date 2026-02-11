namespace SimpleECommerceBackend.Application.UseCases.Order.DTOs;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string ProductImage,
    decimal UnitPrice,
    string Currency,
    int Quantity,
    decimal LineTotal
);
