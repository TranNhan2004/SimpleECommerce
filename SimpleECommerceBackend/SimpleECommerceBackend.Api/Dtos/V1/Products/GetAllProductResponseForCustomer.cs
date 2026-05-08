using SimpleECommerceBackend.Api.Dtos.Common;
using SimpleECommerceBackend.Api.Dtos.Common.Filter;
using SimpleECommerceBackend.Application.Models.Products;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Api.Dtos.V1.Products;

public class ProductItemForCustomer
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public Money CurrentPrice { get; init; }
    public string? DefaultImageUrl { get; init; } = null!;
    public int TotalInStock { get; init; }
    public NavigationResponse Category { get; init; } = null!;
    public NavigationResponse Seller { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

public class GetAllProductsResponseForCustomer : FilterResponse<ProductItemForCustomer>
{
    public static GetAllProductsResponseForCustomer FromResult(GetAllProductsResultForCustomer result)
    {
        return new GetAllProductsResponseForCustomer
        {
            Items = [..result.Items
                .Select(item => new ProductItemForCustomer
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CurrentPrice = item.CurrentPrice,
                    DefaultImageUrl = item.DefaultImageUrl,
                    TotalInStock = item.TotalInStock,
                    Category = new NavigationResponse
                    {
                        Id = item.Category.Id,
                        DisplayName = item.Category.DisplayName
                    },
                    Seller = new NavigationResponse
                    {
                        Id = item.Seller.Id,
                        DisplayName = item.Seller.DisplayName
                    },
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt
                })],
            CurrentPage = result.CurrentPage,
            ItemsPerPage = result.ItemsPerPage,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}