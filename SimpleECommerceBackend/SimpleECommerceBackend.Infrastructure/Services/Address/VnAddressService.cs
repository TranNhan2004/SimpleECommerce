using SimpleECommerceBackend.Application.Interfaces.Services.Address;
using SimpleECommerceBackend.Domain.Exceptions;

using VN.Address;

namespace SimpleECommerceBackend.Infrastructure.Services.Address;

public sealed class VnAddressService : IAddressService
{
    public bool IsValidAddress(string address)
    {
        if (!AddressService.IsValidCharacters(address))
            throw new BusinessException("Address is not valid");
        return true;
    }

    public bool IsValidWard(string province, string ward)
    {
        if (!AddressService.IsValidAddressPair(province, ward))
            throw new BusinessException("Ward is not valid");
        return true;
    }

    public bool IsValidProvince(string province)
    {
        if (!AddressService.IsValidProvince(province))
            throw new BusinessException("Province is not valid");
        return true;
    }
}