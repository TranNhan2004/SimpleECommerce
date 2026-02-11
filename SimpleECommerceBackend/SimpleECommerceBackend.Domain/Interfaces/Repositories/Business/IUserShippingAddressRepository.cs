using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface IUserShippingAddressRepository
{
    Task<IEnumerable<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId);
    Task<CustomerShippingAddress?> FindByIdAsync(Guid id);
    Task<CustomerShippingAddress?> FindDefaultByCustomerIdAsync(Guid customerId);
    CustomerShippingAddress Add(CustomerShippingAddress address);
    CustomerShippingAddress Update(CustomerShippingAddress address);
}