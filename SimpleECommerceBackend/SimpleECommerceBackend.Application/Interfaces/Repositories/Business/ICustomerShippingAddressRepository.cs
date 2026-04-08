using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface ICustomerShippingAddressRepository : IGenericRepository<CustomerShippingAddress>
{
    Task<IReadOnlyList<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId, bool trackChanges = false);
}