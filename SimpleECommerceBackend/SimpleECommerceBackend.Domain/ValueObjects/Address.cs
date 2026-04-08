using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.ValueObjects;

public readonly record struct Address
{
    public Address(string addressLine, string ward, string province)
    {
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new ValidationException(
                AddressErrorCode.AddressLineRequired,
                "Address line is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "AddressLine"
                }
            );

        var trimmedAddress = addressLine.Trim();
        if (trimmedAddress.Length > AddressConstants.AddressLineMaxLength)
            throw new ValidationException(
                AddressErrorCode.AddressLineMaxLengthExceeded,
                $"Address line cannot exceed {AddressConstants.AddressLineMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "AddressLine",
                    ["max"] = AddressConstants.AddressLineMaxLength
                }
            );

        if (string.IsNullOrWhiteSpace(ward))
            throw new ValidationException(
                AddressErrorCode.WardRequired,
                "Ward is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Ward"
                }
            );

        if (string.IsNullOrWhiteSpace(province))
            throw new ValidationException(
                AddressErrorCode.ProvinceRequired,
                "Province is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Province"
                }
            );

        AddressLine = trimmedAddress;
        Ward = ward.Trim();
        Province = province.Trim();
    }

    public string AddressLine { get; } = string.Empty;
    public string Ward { get; } = string.Empty;
    public string Province { get; } = string.Empty;
}
