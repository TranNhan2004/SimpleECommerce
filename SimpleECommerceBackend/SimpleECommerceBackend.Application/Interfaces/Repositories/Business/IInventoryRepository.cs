using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    Task<Inventory?> FindByProductVariantIdAsync(Guid productVariantId, bool trackChanges = false);
}
