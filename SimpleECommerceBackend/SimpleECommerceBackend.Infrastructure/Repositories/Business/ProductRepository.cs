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
    public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
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

        var queryable = pagedProducts.Select(product => new ProductItemForCustomer
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            AverageRating = product.AverageRating,
            TotalRatings = product.TotalRatings,
            Category = new NavigationResult
            {
                Id = product.CategoryId,
                DisplayName = product.Category != null ? product.Category.Name : string.Empty
            },
            Seller = new NavigationResult
            {
                Id = product.SellerId,
                DisplayName = product.Seller != null
                    ? product.Seller.FirstName + " " + product.Seller.LastName
                    : string.Empty
            },
            DefaultImageUrl = product.ProductVariants
                .OrderBy(variant => variant.CurrentPrice.Amount)
                .ThenBy(variant => variant.Id)
                .Select(variant => variant.DefaultImageUrl)
                .First(),
            TotalInStock = product.ProductVariants.Sum(variant => variant.TotalInStock),
            Price = product.ProductVariants
                .OrderBy(variant => variant.CurrentPrice.Amount)
                .ThenBy(variant => variant.Id)
                .Select(variant => variant.CurrentPrice)
                .First(),
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
