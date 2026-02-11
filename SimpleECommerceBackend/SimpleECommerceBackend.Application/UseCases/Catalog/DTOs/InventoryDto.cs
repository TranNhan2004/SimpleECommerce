namespace SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;

public record InventoryDto(
    Guid ProductId,
    int QuantityOnHand,
    int QuantityReserved,
    int AvailableQuantity,
    string? Version,
    DateTimeOffset LastUpdated
);
