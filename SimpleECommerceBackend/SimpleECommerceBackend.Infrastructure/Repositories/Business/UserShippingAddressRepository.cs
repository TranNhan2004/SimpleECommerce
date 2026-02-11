using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class UserShippingAddressRepository : IUserShippingAddressRepository
{
    private readonly AppDbContext _db;

    public async Task<IEnumerable<CustomerShippingAddress>> FindByCustomerIdAsync(Guid customerId)
    {
        return await _db.UserShippingAddresses
            .Where(a => a.CustomerId == customerId && !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<CustomerShippingAddress?> FindByIdAsync(Guid id)
    {
        return await _db.UserShippingAddresses.FindAsync(id);
    }

    public async Task<CustomerShippingAddress?> FindDefaultByCustomerIdAsync(Guid customerId)
    {
        return await _db.UserShippingAddresses
            .FirstOrDefaultAsync(a => a.CustomerId == customerId && a.IsDefault && !a.IsDeleted);
    }

    public CustomerShippingAddress Add(CustomerShippingAddress address)
    {
        _db.UserShippingAddresses.Add(address);
        return address;
    }

    public CustomerShippingAddress Update(CustomerShippingAddress address)
    {
        _db.UserShippingAddresses.Update(address);
        return address;
    }
}
