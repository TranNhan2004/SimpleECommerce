using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;


public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<IReadOnlyList<OrderItem>> FindAllByOrderIdAsync(Guid orderId, bool trackChanges = false)
    {
        return await base.FindAllByConditionAsync(
            q => q.Where(oi => oi.OrderId == orderId),
            trackChanges
        );
    }
}