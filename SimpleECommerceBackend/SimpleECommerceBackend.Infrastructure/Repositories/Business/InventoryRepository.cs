using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<Inventory?> FindByProductVariantIdAsync(Guid productVariantId, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q.Where(i => i.ProductVariantId == productVariantId),
            trackChanges: trackChanges
        );
    }
}
