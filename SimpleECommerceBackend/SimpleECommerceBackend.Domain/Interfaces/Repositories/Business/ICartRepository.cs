using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface ICartRepository
{
    Task<Cart?> FindByCustomerIdAsync(Guid customerId);
    Cart Add(Cart cart);
    Cart Update(Cart cart);
    Cart Delete(Cart cart);
}
