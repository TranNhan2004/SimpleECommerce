using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<Payment?> FindByOrderIdAsync(Guid orderId, bool trackChanges = false)
    {
        return await base.FindFirstByConditionAsync(
            q => q.Where(p => p.OrderId == orderId),
            trackChanges
        );
    }

}