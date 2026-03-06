using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface ICustomerShippingAddressRepository
{
    Task<IReadOnlyList<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId);
    Task<CustomerShippingAddress?> FindByIdAsync(Guid id);
    CustomerShippingAddress Add(CustomerShippingAddress address);
    CustomerShippingAddress Update(CustomerShippingAddress address);
    CustomerShippingAddress Delete(CustomerShippingAddress address);
}