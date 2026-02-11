using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class SellerShopRepository : ISellerShopRepository
{
    private readonly AppDbContext _db;
    
    public async Task<IEnumerable<SellerShop>> FindAllAsync()
    {
        return await _db.SellerShops.ToListAsync();
    }

    public async Task<SellerShop?> FindByIdAsync(Guid id)
    {
        return await _db.SellerShops.Include(ss => ss.SellerWarehouses)
            .FirstOrDefaultAsync(ss => ss.Id == id);
    }

    public async Task<SellerShop?> FindBySellerIdAsync(Guid sellerId)
    {
        return await _db.SellerShops.Include(ss => ss.SellerWarehouses)
            .FirstOrDefaultAsync(ss => ss.SellerId == sellerId);
    }

    public SellerShop Add(SellerShop sellerShop)
    {
        _db.SellerShops.Add(sellerShop);
        return sellerShop;
    }

    public SellerShop Update(SellerShop sellerShop)
    {
        _db.SellerShops.Update(sellerShop);
        return sellerShop;
    }

    public SellerShop Delete(SellerShop sellerShop)
    {
        _db.SellerShops.Remove(sellerShop);
        return sellerShop;
    }
}