using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> FindByIdAsync(Guid id);
    Task<Payment?> FindByOrderIdAsync(Guid orderId);
    Task<IReadOnlyList<Payment>> FindAllAsync();
    Payment Add(Payment payment);
    Payment Update(Payment payment);
}
