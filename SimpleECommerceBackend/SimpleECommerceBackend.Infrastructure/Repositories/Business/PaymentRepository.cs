using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;

    public async Task<Payment?> FindByIdAsync(Guid id)
    {
        return await _db.Payments.FindAsync(id);
    }

    public async Task<Payment?> FindByOrderIdAsync(Guid orderId)
    {
        return await _db.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<IEnumerable<Payment>> FindAllAsync()
    {
        return await _db.Payments
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public Payment Add(Payment payment)
    {
        _db.Payments.Add(payment);
        return payment;
    }

    public Payment Update(Payment payment)
    {
        _db.Payments.Update(payment);
        return payment;
    }
}
