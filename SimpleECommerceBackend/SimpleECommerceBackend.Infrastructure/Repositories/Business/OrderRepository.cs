using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<Order?> FindByCodeAsync(string code, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q
                .Include(o => o.OrderItems)
                .Where(o => o.Code == code),
            trackChanges
        );
    }

    public async Task<IReadOnlyList<Order>> FindAllByCustomerIdAsync(Guid customerId, bool trackChanges = false)
    {
        return await base.FindAllByConditionAsync(
            q => q
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt),
            trackChanges
        );
    }
}