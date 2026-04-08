using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> FindByCodeAsync(string code, bool trackChanges = false);
    Task<IReadOnlyList<Order>> FindAllByCustomerIdAsync(Guid customerId, bool trackChanges = false);
}