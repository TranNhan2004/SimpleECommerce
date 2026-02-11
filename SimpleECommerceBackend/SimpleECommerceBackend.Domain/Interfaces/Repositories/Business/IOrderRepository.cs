using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface IOrderRepository
{
    Task<Order?> FindByIdAsync(Guid id);
    Task<Order?> FindByCodeAsync(string code);
    Task<IEnumerable<Order>> FindByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Order>> FindAllAsync();
    Order Add(Order order);
    Order Update(Order order);
}
