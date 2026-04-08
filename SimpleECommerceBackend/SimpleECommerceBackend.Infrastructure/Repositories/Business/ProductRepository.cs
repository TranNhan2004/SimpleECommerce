using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;


public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public override async Task<Product?> FindByIdAsync(Guid id, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.ProductImages)
                .Where(p => p.Id == id),
            trackChanges
        );
    }
}