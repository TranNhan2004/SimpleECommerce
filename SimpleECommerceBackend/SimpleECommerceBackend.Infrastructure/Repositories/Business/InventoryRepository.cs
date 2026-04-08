using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public partial class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<Inventory?> FindByProductIdAsync(Guid productId, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q.Where(i => i.ProductId == productId),
            trackChanges: trackChanges
        );
    }
}