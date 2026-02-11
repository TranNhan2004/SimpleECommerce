using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class CartItemRepository : ICartItemRepository
{
    private readonly AppDbContext _db;

    public async Task<IEnumerable<CartItem>> FindByCartIdAsync(Guid cartId)
    {
        return await _db.CartItems
            .Where(ci => ci.CartId == cartId)
            .ToListAsync();
    }

    public async Task<CartItem?> FindByIdAsync(Guid id)
    {
        return await _db.CartItems.FindAsync(id);
    }

    public CartItem Add(CartItem cartItem)
    {
        _db.CartItems.Add(cartItem);
        return cartItem;
    }

    public CartItem Update(CartItem cartItem)
    {
        _db.CartItems.Update(cartItem);
        return cartItem;
    }

    public CartItem Delete(CartItem cartItem)
    {
        _db.CartItems.Remove(cartItem);
        return cartItem;
    }
}