using SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;

namespace SimpleECommerceBackend.Application.UseCases.Cart.DTOs;

public record CartItemDto(
    Guid ProductId,
    string ProductName,
    string ProductImage, // Main image url
    decimal UnitPrice,
    string Currency,
    int Quantity,
    decimal LineTotal
);
