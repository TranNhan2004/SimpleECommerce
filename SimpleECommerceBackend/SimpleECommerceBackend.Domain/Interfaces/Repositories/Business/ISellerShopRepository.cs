using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface ISellerShopRepository
{
    Task<IEnumerable<SellerShop>> FindAllAsync();
    Task<SellerShop?> FindByIdAsync(Guid id);
    Task<SellerShop?> FindBySellerIdAsync(Guid sellerId);
    SellerShop Add(SellerShop sellerShop);
    SellerShop Update(SellerShop sellerShop);
    SellerShop Delete(SellerShop sellerShop);
}