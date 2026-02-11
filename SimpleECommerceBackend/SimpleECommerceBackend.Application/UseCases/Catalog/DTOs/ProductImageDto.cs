namespace SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;

public record ProductImageDto(
    Guid ProductId,
    string ImageUrl,
    int DisplayOrder,
    bool IsDisplayed,
    string? Description
);
