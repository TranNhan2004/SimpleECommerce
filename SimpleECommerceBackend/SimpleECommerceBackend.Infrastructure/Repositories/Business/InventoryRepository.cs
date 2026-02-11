using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _db;

    public async Task<Inventory?> FindByProductIdAsync(Guid productId)
    {
        return await _db.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public Inventory Add(Inventory inventory)
    {
        _db.Inventories.Add(inventory);
        return inventory;
    }

    public Inventory Update(Inventory inventory)
    {
        _db.Inventories.Update(inventory);
        return inventory;
    }
}
