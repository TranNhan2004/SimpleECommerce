using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class CustomerShippingAddress : EntityBase, ICreatedTime, IUpdatedTime, ISoftDeletable
{
    private CustomerShippingAddress() {}

    private CustomerShippingAddress(
        string recipientName,
        string recipientPhoneNumber,
        Address recipientAddress, 
        bool isDefault)
    {
        SetRecipientName(recipientName);
        SetRecipientPhoneNumber(recipientPhoneNumber);
        SetRecipientAddress(recipientAddress);
        SetIsDefault(isDefault);
    }
    
    public string RecipientName { get; private set; } = string.Empty;
    public string RecipientPhoneNumber { get; private set; } = string.Empty;
    public Address RecipientAddress { get; private set; }
    
    public bool IsDefault { get; private set; }
    
    public Guid CustomerId { get; private set; }
    public UserProfile? Customer { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set;  }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public void SoftDelete()
    {
        if (IsDeleted)
            throw new DomainException("Address has beed deleted");

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
    
    public void SetRecipientName(string recipientName)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
            throw new DomainException("Recipient name is required");

        var trimmedName = recipientName.Trim();

        if (trimmedName.Length > ShippingAddressConstants.RecipientNameMaxLength)
            throw new DomainException(
                $"Recipient name cannot exceed {ShippingAddressConstants.RecipientNameMaxLength} characters");

        RecipientName = trimmedName;
    }

    public void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientPhoneNumber))
            throw new DomainException("Recipient phone number is required");

        var trimmedRecipientPhoneNumber = recipientPhoneNumber.Trim();

        if (trimmedRecipientPhoneNumber.Length > CommonConstants.PhoneNumberMaxLength)
            throw new DomainException(
                $"Recipient phone number cannot exceed {CommonConstants.PhoneNumberMaxLength} characters");

        RecipientPhoneNumber = trimmedRecipientPhoneNumber;
    }

    public void SetRecipientAddress(Address recipientAddress)
    {
        RecipientAddress = recipientAddress;
    }

    private void SetIsDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }

    public void MarkAsDefault()
    {
        if (IsDefault)
            throw new DomainException("Address is default");

        SetIsDefault(true);
    }

    public void RemoveDefault()
    {
        if (!IsDefault)
            throw new DomainException("Address is not default");

        SetIsDefault(false);
    }

    public static CustomerShippingAddress Create(
        string recipientName,
        string recipientPhoneNumber,
        Address recipientAddress
    )
    {
        return new CustomerShippingAddress(recipientName, recipientPhoneNumber, recipientAddress, false);
    }

}