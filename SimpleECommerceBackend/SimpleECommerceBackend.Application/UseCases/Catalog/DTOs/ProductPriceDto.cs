namespace SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;

public record ProductPriceDto(
    Guid ProductId,
    decimal Amount,
    string Currency,
    DateTime EffectiveFrom
);
