using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> FindByOrderIdAsync(Guid orderId, bool trackChanges = false);
}