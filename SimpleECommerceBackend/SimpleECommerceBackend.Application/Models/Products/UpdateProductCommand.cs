using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Application.Models.Products;

public class UpdateProductVariantCommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Money CurrentPrice { get; init; }
    public int TotalInStock { get; init; }
    public string? DefaultImageUrl { get; init; }
    public ProductInvariantStatus Status { get; init; }
}

public class UpdateProductCommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Guid CategoryId { get; init; }
    public Guid SellerId { get; init; }
    public IReadOnlyList<UpdateProductVariantCommand> Variants { get; init; } = [];
}
