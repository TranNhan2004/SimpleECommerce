using SimpleECommerceBackend.Application.Interfaces.Services.Address;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using VN.Address;

namespace SimpleECommerceBackend.Infrastructure.Services.Address;

public class VnAddressService : IAddressService
{
    public bool IsValidAddress(string address)
    {
        if (!AddressService.IsValidCharacters(address))
            throw new ValidationException(
                VnAddressErrorCode.AddressInvalid,
                "Address is not valid",
                new Dictionary<string, object?>
                {
                    ["field"] = "Address"
                }
            );
        return true;
    }

    public bool IsValidWard(string province, string ward)
    {
        if (!AddressService.IsValidAddressPair(province, ward))
            throw new ValidationException(
                VnAddressErrorCode.WardInvalid,
                "Ward is not valid",
                new Dictionary<string, object?>
                {
                    ["field"] = "Ward"
                }
            );
        return true;
    }

    public bool IsValidProvince(string province)
    {
        if (!AddressService.IsValidProvince(province))
            throw new ValidationException(
                VnAddressErrorCode.ProvinceInvalid,
                "Province is not valid",
                new Dictionary<string, object?>
                {
                    ["field"] = "Province"
                }
            );
        return true;
    }
}