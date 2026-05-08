using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Application.Models.Products;

public class UpdateProductCommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Money CurrentPrice { get; init; }
    public ProductStatus Status { get; init; }
    public Guid CategoryId { get; init; }
    public Guid SellerId { get; init; }
}
