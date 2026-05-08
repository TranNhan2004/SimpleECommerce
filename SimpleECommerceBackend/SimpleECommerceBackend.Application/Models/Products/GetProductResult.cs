using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Products;

public class GetProductResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal CurrentPriceAmount { get; init; }
    public string CurrentPriceCurrency { get; init; } = null!;
    public int TotalInStock { get; init; }
    public ProductStatus Status { get; init; }
    public Guid CategoryId { get; init; }
    public Guid SellerId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public static GetProductResult FromEntity(Product entity)
    {
        return new GetProductResult
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CurrentPriceAmount = entity.CurrentPrice.Amount,
            CurrentPriceCurrency = entity.CurrentPrice.Currency,
            TotalInStock = entity.TotalInStock,
            Status = entity.Status,
            CategoryId = entity.CategoryId,
            SellerId = entity.SellerId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
