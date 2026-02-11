using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface IPaymentRepository
{
    Task<Payment?> FindByIdAsync(Guid id);
    Task<Payment?> FindByOrderIdAsync(Guid orderId);
    Task<IEnumerable<Payment>> FindAllAsync();
    Payment Add(Payment payment);
    Payment Update(Payment payment);
}
