using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<Cart?> FindByCustomerIdAsync(Guid customerId, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            c => c.CustomerId == customerId,
            q => q.Include(c => c.CartItems),
            trackChanges
        );
    }
}