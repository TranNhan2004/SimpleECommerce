using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;


public class SellerShopRepository : GenericRepository<SellerShop>, ISellerShopRepository
{
    public SellerShopRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public override async Task<SellerShop?> FindByIdAsync(Guid id, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q
                .Include(ss => ss.SellerWarehouses)
                .Where(ss => ss.Id == id),
            trackChanges
        );
    }

    public async Task<SellerShop?> FindBySellerIdAsync(Guid sellerId, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q
                .Include(ss => ss.SellerWarehouses)
                .Where(ss => ss.SellerId == sellerId),
            trackChanges
        );
    }
}