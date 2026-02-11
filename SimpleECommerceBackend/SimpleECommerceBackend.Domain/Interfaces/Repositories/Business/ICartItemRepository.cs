using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface ICartItemRepository
{
    Task<IEnumerable<CartItem>> FindByCartIdAsync(Guid cartId);
    Task<CartItem?> FindByIdAsync(Guid id);
    CartItem Add(CartItem cartItem);
    CartItem Update(CartItem cartItem);
    CartItem Delete(CartItem cartItem);
}