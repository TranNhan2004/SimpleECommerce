using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface IInventoryRepository
{
    Task<Inventory?> FindByProductIdAsync(Guid productId);
    Inventory Add(Inventory inventory);
    Inventory Update(Inventory inventory);
}
