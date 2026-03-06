using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories;

[AutoConstructor]
public partial class OrderItemRepository : IOrderItemRepository
{
    private readonly AppDbContext _db;

    public async Task<IReadOnlyList<OrderItem>> FindByOrderIdAsync(Guid orderId)
    {
        return await _db.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<OrderItem?> FindByIdAsync(Guid id)
    {
        return await _db.OrderItems.FindAsync(id);
    }

    public OrderItem Add(OrderItem orderItem)
    {
        _db.OrderItems.Add(orderItem);
        return orderItem;
    }

    public OrderItem Update(OrderItem orderItem)
    {
        _db.OrderItems.Update(orderItem);
        return orderItem;
    }

    public OrderItem Delete(OrderItem orderItem)
    {
        _db.OrderItems.Remove(orderItem);
        return orderItem;
    }
}