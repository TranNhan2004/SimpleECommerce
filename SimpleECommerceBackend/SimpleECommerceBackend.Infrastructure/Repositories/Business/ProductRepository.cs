using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Models.Common;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Application.Models.Products;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Infrastructure.Extensions;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;


public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<FilterResult<ProductItemForCustomer>> FindAllWithFilterForCustomerAsync(FilterQuery<Product> query)
    {
        var baseQuery = QueryAllByCondition(q => q.Where(product =>
            product.Status == ProductStatus.Active
            && product.ProductVariants.Any()));
        var filteredQuery = baseQuery.ApplyFiltering(query);
        var sortedQuery = filteredQuery.ApplySorting(query);
        var totalItems = await sortedQuery.CountAsync();
        var pagedProducts = sortedQuery.ApplyPaging(query);

        var queryable = pagedProducts
            .Select(product => new
            {
                product.Id,
                product.Name,
                product.Description,
                product.AverageRating,
                product.TotalRatings,
                product.CategoryId,
                CategoryName = product.Category != null ? product.Category.Name : string.Empty,
                product.SellerId,
                SellerDisplayName = product.Seller != null
                    ? product.Seller.FirstName + " " + product.Seller.LastName
                    : string.Empty,
                LowestPricedVariant = product.ProductVariants
                    .Where(variant => variant.Status == ProductInvariantStatus.Active)
                    .OrderBy(variant => variant.CurrentPrice.Amount)
                    .Select(variant => new
                    {
                        variant.DefaultImageUrl,
                        variant.CurrentPrice
                    })
                    .First(),
                TotalInStock = product.ProductVariants.Sum(variant => variant.TotalInStock),
                product.CreatedAt,
                product.UpdatedAt
            })
            .Select(product => new ProductItemForCustomer
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                AverageRating = product.AverageRating,
                TotalRatings = product.TotalRatings,
                Category = new NavigationResult
                {
                    Id = product.CategoryId,
                    DisplayName = product.CategoryName
                },
                Seller = new NavigationResult
                {
                    Id = product.SellerId,
                    DisplayName = product.SellerDisplayName
                },
                DefaultImageUrl = product.LowestPricedVariant.DefaultImageUrl,
                TotalInStock = product.TotalInStock,
                Price = product.LowestPricedVariant.CurrentPrice,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            });

        return new FilterResult<ProductItemForCustomer>
        {
            Items = [.. await queryable.ToListAsync()],
            CurrentPage = query.CurrentPage,
            ItemsPerPage = query.ItemsPerPage,
            TotalItems = totalItems,
            TotalPages = totalItems == 0
                ? 0
                : (int)Math.Ceiling(totalItems / (double)query.ItemsPerPage)
        };
    }

    public override async Task<Product?> FindByIdAsync(Guid id, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.ProductVariantImages)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.ProductVariantPrices)
                .Include(p => p.Reviews)
                    .ThenInclude(review => review.Responses),
            trackChanges
        );
    }
}
