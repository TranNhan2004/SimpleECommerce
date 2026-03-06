using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> FindByCustomerIdAsync(Guid customerId);
    Cart Add(Cart cart);
    Cart Update(Cart cart);
    Cart Delete(Cart cart);
}
