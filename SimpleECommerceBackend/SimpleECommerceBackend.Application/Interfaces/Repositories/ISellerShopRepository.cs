using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface ISellerShopRepository
{
    Task<IReadOnlyList<SellerShop>> FindAllAsync();
    Task<SellerShop?> FindByIdAsync(Guid id);
    Task<SellerShop?> FindBySellerIdAsync(Guid sellerId);
    SellerShop Add(SellerShop sellerShop);
    SellerShop Update(SellerShop sellerShop);
    SellerShop Delete(SellerShop sellerShop);
}