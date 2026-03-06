using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task<IReadOnlyList<OrderItem>> FindByOrderIdAsync(Guid orderId);
    Task<OrderItem?> FindByIdAsync(Guid id);
    OrderItem Add(OrderItem orderItem);
    OrderItem Update(OrderItem orderItem);
    OrderItem Delete(OrderItem orderItem);
}