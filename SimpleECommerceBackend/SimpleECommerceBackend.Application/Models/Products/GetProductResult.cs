using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Models.Products;

public class GetProductResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public double AverageRating { get; init; }
    public int TotalRatings { get; init; }
    public Guid CategoryId { get; init; }
    public Guid SellerId { get; init; }
    public IReadOnlyList<ProductVariantItemForCustomer> Variants { get; init; } = [];
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public static GetProductResult FromEntity(Product entity)
    {
        return new GetProductResult
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            AverageRating = entity.AverageRating,
            TotalRatings = entity.TotalRatings,
            CategoryId = entity.CategoryId,
            SellerId = entity.SellerId,
            Variants = [.. entity.ProductVariants.Select(ProductVariantItemForCustomer.FromEntity)],
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
