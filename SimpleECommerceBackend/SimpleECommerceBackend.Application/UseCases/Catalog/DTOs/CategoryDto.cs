namespace SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    string Status
);
