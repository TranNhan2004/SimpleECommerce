using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IInventoryRepository
{
    Task<Inventory?> FindByProductIdAsync(Guid productId);
    Inventory Add(Inventory inventory);
    Inventory Update(Inventory inventory);
}
