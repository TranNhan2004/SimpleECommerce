namespace SimpleECommerceBackend.Application.UseCases.Cart.DTOs;

public record CartDto(
    Guid CustomerId,
    List<CartItemDto> Items,
    decimal TotalAmount,
    string Currency
);
