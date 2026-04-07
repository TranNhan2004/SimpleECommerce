using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
    Task<IReadOnlyList<OrderItem>> FindByOrderIdAsync(Guid orderId);
}