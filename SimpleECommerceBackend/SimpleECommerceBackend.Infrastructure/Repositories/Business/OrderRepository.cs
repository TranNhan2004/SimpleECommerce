using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public partial class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public async Task<Order?> FindByIdAsync(Guid id)
    {
        return await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> FindByCodeAsync(string code)
    {
        return await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Code == code);
    }

    public async Task<IReadOnlyList<Order>> FindByCustomerIdAsync(Guid customerId)
    {
        return await _db.Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> FindAllAsync()
    {
        return await _db.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public Order Add(Order order)
    {
        _db.Orders.Add(order);
        return order;
    }

    public Order Update(Order order)
    {
        _db.Orders.Update(order);
        return order;
    }
}