using SimpleECommerceBackend.Application.Models.Common;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Application.Models.Products;

public class ProductVariantItemForCustomer
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Money CurrentPrice { get; init; }
    public string? DefaultImageUrl { get; init; }
    public int TotalInStock { get; init; }
    public ProductInvariantStatus Status { get; init; }

    public static ProductVariantItemForCustomer FromEntity(ProductVariant entity)
    {
        return new ProductVariantItemForCustomer
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CurrentPrice = entity.CurrentPrice,
            DefaultImageUrl = entity.DefaultImageUrl,
            TotalInStock = entity.TotalInStock,
            Status = entity.Status
        };
    }
}

public class ProductItemForCustomer
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public double AverageRating { get; init; }
    public int TotalRatings { get; init; }
    public NavigationResult Category { get; init; } = null!;
    public NavigationResult Seller { get; init; } = null!;
    public string? DefaultImageUrl { get; init; }
    public int TotalInStock { get; init; }
    public Money Price { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public static ProductItemForCustomer FromEntity(Product entity)
    {
        var lowestPricedVariant = entity.ProductVariants
            .OrderBy(variant => variant.CurrentPrice.Amount)
            .ThenBy(variant => variant.Id)
            .First();

        return new ProductItemForCustomer
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            AverageRating = entity.AverageRating,
            TotalRatings = entity.TotalRatings,
            Category = new NavigationResult
            {
                Id = entity.CategoryId,
                DisplayName = entity.Category?.Name ?? string.Empty
            },
            Seller = new NavigationResult
            {
                Id = entity.SellerId,
                DisplayName = (entity.Seller?.FirstName + " " + entity.Seller?.LastName) ?? string.Empty
            },
            DefaultImageUrl = lowestPricedVariant.DefaultImageUrl,
            TotalInStock = entity.ProductVariants.Sum(variant => variant.TotalInStock),
            Price = lowestPricedVariant.CurrentPrice,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

public class GetAllProductsResultForCustomer : FilterResult<ProductItemForCustomer>
{
    public static GetAllProductsResultForCustomer FromFilterResult(FilterResult<ProductItemForCustomer> filterResult)
    {
        return new GetAllProductsResultForCustomer
        {
            Items = filterResult.Items,
            CurrentPage = filterResult.CurrentPage,
            ItemsPerPage = filterResult.ItemsPerPage,
            TotalItems = filterResult.TotalItems,
            TotalPages = filterResult.TotalPages
        };
    }
}
