using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class CustomerShippingAddressRepository : GenericRepository<CustomerShippingAddress>, ICustomerShippingAddressRepository
{
    public CustomerShippingAddressRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<IReadOnlyList<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId, bool trackChanges = false)
    {
        return await base.FindAllByConditionAsync(
            q => q.Where(c => c.CustomerId == customerId),
            trackChanges
        );
    }
}