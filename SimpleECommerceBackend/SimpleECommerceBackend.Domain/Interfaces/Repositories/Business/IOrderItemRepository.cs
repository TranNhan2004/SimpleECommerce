using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface IOrderItemRepository
{
    Task<IEnumerable<OrderItem>> FindByOrderIdAsync(Guid orderId);
    Task<OrderItem?> FindByIdAsync(Guid id);
    OrderItem Add(OrderItem orderItem);
    OrderItem Update(OrderItem orderItem);
    OrderItem Delete(OrderItem orderItem);
}