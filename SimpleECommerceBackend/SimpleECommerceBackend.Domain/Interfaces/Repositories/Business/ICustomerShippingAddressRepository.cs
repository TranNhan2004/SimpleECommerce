using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface ICustomerShippingAddressRepository
{
    Task<IEnumerable<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId);
    Task<CustomerShippingAddress?> FindByIdAsync(Guid id);
    CustomerShippingAddress Add(CustomerShippingAddress address);
    CustomerShippingAddress Update(CustomerShippingAddress address);
    CustomerShippingAddress Delete(CustomerShippingAddress address);
}