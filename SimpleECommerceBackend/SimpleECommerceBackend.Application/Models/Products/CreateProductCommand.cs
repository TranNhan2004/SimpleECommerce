using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Application.Models.Products;

public class CreateProductCommand
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Money CurrentPrice { get; init; }
    public Guid CategoryId { get; init; }
    public Guid SellerId { get; init; }
}
