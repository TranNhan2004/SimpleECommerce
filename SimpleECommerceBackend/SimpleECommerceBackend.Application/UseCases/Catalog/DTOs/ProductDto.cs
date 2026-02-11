namespace SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal CurrentPrice,
    string Currency,
    string Status,
    Guid CategoryId,
    string CategoryName,
    Guid SellerId,
    int AvailableStock,
    List<ProductImageDto> Images,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
