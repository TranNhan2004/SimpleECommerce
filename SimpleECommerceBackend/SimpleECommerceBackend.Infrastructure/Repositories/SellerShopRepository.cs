using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories;

[AutoConstructor]
public partial class SellerShopRepository : ISellerShopRepository
{
    private readonly AppDbContext _db;

    public async Task<IReadOnlyList<SellerShop>> FindAllAsync()
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