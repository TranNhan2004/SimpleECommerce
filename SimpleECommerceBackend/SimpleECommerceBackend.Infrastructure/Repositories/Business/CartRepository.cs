using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public async Task<Cart?> FindByCustomerIdAsync(Guid customerId)
    {
        return await _db.Carts
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public Cart Add(Cart cart)
    {
        _db.Carts.Add(cart);
        return cart;
    }

    public Cart Update(Cart cart)
    {
        _db.Carts.Update(cart);
        return cart;
    }

    public Cart Delete(Cart cart)
    {
        _db.Carts.Remove(cart);
        return cart;
    }
}