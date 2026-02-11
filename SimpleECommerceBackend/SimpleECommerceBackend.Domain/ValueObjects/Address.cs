using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.ValueObjects;

public readonly record struct Address
{
    public Address(string addressLine, string ward, string province)
    {
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new DomainException("Address line is required");

        var trimmedAddress = addressLine.Trim();
        if (trimmedAddress.Length > AddressConstants.AddressLineMaxLength)
            throw new DomainException(
                $"Address line cannot exceed {AddressConstants.AddressLineMaxLength} characters");

        if (string.IsNullOrWhiteSpace(ward))
            throw new DomainException("Ward is required");

        if (string.IsNullOrWhiteSpace(province))
            throw new DomainException("Province is required");

        AddressLine = trimmedAddress;
        Ward = ward.Trim();
        Province = province.Trim();
    }

    public string AddressLine { get; } = string.Empty;
    public string Ward { get; } = string.Empty;
    public string Province { get; } = string.Empty;
}