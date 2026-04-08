using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> FindByCustomerIdAsync(Guid customerId, bool trackChanges = false);
}