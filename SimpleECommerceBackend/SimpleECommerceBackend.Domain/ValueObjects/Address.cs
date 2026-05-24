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
                AddressErrorCodes.AddressLineRequired,
                "Address line is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "AddressLine"
                }
            );

        var trimmedAddress = addressLine.Trim();
        if (trimmedAddress.Length > AddressValidationRules.AddressLineMaxLength)
            throw new ValidationException(
                AddressErrorCodes.AddressLineMaxLengthExceeded,
                $"Address line cannot exceed {AddressValidationRules.AddressLineMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "AddressLine",
                    ["max"] = AddressValidationRules.AddressLineMaxLength
                }
            );

        if (string.IsNullOrWhiteSpace(ward))
            throw new ValidationException(
                AddressErrorCodes.WardRequired,
                "Ward is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Ward"
                }
            );

        if (string.IsNullOrWhiteSpace(province))
            throw new ValidationException(
                AddressErrorCodes.ProvinceRequired,
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
