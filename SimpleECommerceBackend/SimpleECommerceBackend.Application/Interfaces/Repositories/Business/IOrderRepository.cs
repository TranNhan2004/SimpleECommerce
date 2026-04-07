using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> FindByCodeAsync(string code);
    Task<IReadOnlyList<Order>> FindByCustomerIdAsync(Guid customerId);
}