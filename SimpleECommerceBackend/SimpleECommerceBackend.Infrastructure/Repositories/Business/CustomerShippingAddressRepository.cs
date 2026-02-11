using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class CustomerShippingAddressRepository : ICustomerShippingAddressRepository
{
    private readonly AppDbContext _db;

    public async Task<IEnumerable<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId)
    {
        return await _db.CustomerShippingAddresses
            .Where(a => a.CustomerId == customerId && !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<CustomerShippingAddress?> FindByIdAsync(Guid id)
    {
        return await _db.CustomerShippingAddresses.FindAsync(id);
    }

    public CustomerShippingAddress Add(CustomerShippingAddress address)
    {
        _db.CustomerShippingAddresses.Add(address);
        return address;
    }

    public CustomerShippingAddress Update(CustomerShippingAddress address)
    {
        _db.CustomerShippingAddresses.Update(address);
        return address;
    }

    public CustomerShippingAddress Delete(CustomerShippingAddress address)
    {
        _db.CustomerShippingAddresses.Remove(address);
        return address;
    }
}
