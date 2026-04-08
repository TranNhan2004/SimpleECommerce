using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface ISellerShopRepository : IGenericRepository<SellerShop>
{
    Task<SellerShop?> FindBySellerIdAsync(Guid sellerId, bool trackChanges = false);
}