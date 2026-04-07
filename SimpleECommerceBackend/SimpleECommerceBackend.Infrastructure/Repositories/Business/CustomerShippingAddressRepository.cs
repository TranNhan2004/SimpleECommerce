using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class CustomerShippingAddressRepository : GenericRepository<CustomerShippingAddress>,
    ICustomerShippingAddressRepository
{
    public CustomerShippingAddressRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<IReadOnlyList<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId)
    {
        return await base.FindAllByConditionAsync(csa => csa.CustomerId == customerId);
    }
}