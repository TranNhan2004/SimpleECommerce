using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Application.Models.Products;

public class CreateProductVariantCommand
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Money CurrentPrice { get; init; }
    public int TotalInStock { get; init; }
    public string? DefaultImageUrl { get; init; }
    public ProductInvariantStatus Status { get; init; }
}

public class CreateProductCommand
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Guid CategoryId { get; init; }
    public Guid SellerId { get; init; }
    public IReadOnlyList<CreateProductVariantCommand> Variants { get; init; } = [];
}
