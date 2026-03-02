using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IInventoryRepository
{
    Task<Inventory?> FindByProductIdAsync(Guid productId);
    Inventory Add(Inventory inventory);
    Inventory Update(Inventory inventory);
}
