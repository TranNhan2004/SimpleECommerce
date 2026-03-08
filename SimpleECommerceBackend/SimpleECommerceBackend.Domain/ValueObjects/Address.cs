using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.ValueObjects;

public readonly record struct Address
{
    public Address(string addressLine, string ward, string province)
    {
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new BusinessException("Address line is required");

        var trimmedAddress = addressLine.Trim();
        if (trimmedAddress.Length > AddressConstants.AddressLineMaxLength)
            throw new BusinessException(
                $"Address line cannot exceed {AddressConstants.AddressLineMaxLength} characters");

        if (string.IsNullOrWhiteSpace(ward))
            throw new BusinessException("Ward is required");

        if (string.IsNullOrWhiteSpace(province))
            throw new BusinessException("Province is required");

        AddressLine = trimmedAddress;
        Ward = ward.Trim();
        Province = province.Trim();
    }

    public string AddressLine { get; } = string.Empty;
    public string Ward { get; } = string.Empty;
    public string Province { get; } = string.Empty;
}