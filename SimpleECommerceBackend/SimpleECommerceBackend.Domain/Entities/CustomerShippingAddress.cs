using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities;

public class CustomerShippingAddress : Entity, ICreatedTrackable, IUpdatedTrackable, ISoftDeleteTrackable
{
    private CustomerShippingAddress()
    {
    }

    private CustomerShippingAddress(
        string recipientName,
        string recipientPhoneNumber,
        Address recipientAddress,
        bool isDefault
    )
    {
        SetId(Guid.NewGuid());
        SetRecipientName(recipientName);
        SetRecipientPhoneNumber(recipientPhoneNumber);
        SetRecipientAddress(recipientAddress);
        SetIsDefault(isDefault);
    }

    public string RecipientName { get; private set; } = null!;
    public string RecipientPhoneNumber { get; private set; } = null!;
    public Address RecipientAddress { get; private set; }

    public bool IsDefault { get; private set; }

    public Guid CustomerId { get; private set; }
    public UserProfile? Customer { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new ValidationException(
                CustomerShippingAddressErrorCode.AlreadyDeleted,
                "Address has beed deleted",
                new Dictionary<string, object?>
                {
                    ["field"] = "Address"
                }
            );

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void SetRecipientName(string recipientName)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
            throw new ValidationException(
                CustomerShippingAddressErrorCode.RecipientNameRequired,
                "Recipient name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName"
                }
            );

        var trimmedName = recipientName.Trim();

        if (trimmedName.Length > ShippingAddressConstants.RecipientNameMaxLength)
            throw new ValidationException(
                CustomerShippingAddressErrorCode.RecipientNameMaxLengthExceeded,
                $"Recipient name cannot exceed {ShippingAddressConstants.RecipientNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName",
                    ["max"] = ShippingAddressConstants.RecipientNameMaxLength
                }
            );

        RecipientName = trimmedName;
    }

    public void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientPhoneNumber))
            throw new ValidationException(
                CustomerShippingAddressErrorCode.RecipientPhoneNumberRequired,
                "Recipient phone number is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber"
                }
            );

        var trimmedRecipientPhoneNumber = recipientPhoneNumber.Trim();

        if (trimmedRecipientPhoneNumber.Length > CommonConstants.PhoneNumberMaxLength)
            throw new ValidationException(
                CustomerShippingAddressErrorCode.RecipientPhoneNumberMaxLengthExceeded,
                $"Recipient phone number cannot exceed {CommonConstants.PhoneNumberMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber",
                    ["max"] = CommonConstants.PhoneNumberMaxLength
                }
            );

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
            throw new ValidationException(
                CustomerShippingAddressErrorCode.AlreadyDefault,
                "Address is default",
                new Dictionary<string, object?>
                {
                    ["field"] = "Address"
                }
            );

        SetIsDefault(true);
    }

    public void RemoveDefault()
    {
        if (!IsDefault)
            throw new ValidationException(
                CustomerShippingAddressErrorCode.NotDefault,
                "Address is not default",
                new Dictionary<string, object?>
                {
                    ["field"] = "Address"
                }
            );

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
