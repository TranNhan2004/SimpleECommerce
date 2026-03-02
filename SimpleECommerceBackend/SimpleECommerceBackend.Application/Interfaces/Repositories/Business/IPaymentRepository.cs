using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IPaymentRepository
{
    Task<Payment?> FindByIdAsync(Guid id);
    Task<Payment?> FindByOrderIdAsync(Guid orderId);
    Task<IReadOnlyList<Payment>> FindAllAsync();
    Payment Add(Payment payment);
    Payment Update(Payment payment);
}
