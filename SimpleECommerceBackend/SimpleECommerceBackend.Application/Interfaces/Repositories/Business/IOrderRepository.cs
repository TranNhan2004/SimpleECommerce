using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IOrderRepository
{
    Task<Order?> FindByIdAsync(Guid id);
    Task<Order?> FindByCodeAsync(string code);
    Task<IReadOnlyList<Order>> FindByCustomerIdAsync(Guid customerId);
    Task<IReadOnlyList<Order>> FindAllAsync();
    Order Add(Order order);
    Order Update(Order order);
}
