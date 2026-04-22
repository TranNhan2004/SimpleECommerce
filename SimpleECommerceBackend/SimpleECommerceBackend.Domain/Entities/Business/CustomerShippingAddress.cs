using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

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
                CustomerShippingAddressErrorCodes.AlreadyDeleted,
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
                CustomerShippingAddressErrorCodes.RecipientNameRequired,
                "Recipient name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName"
                }
            );

        var trimmedName = recipientName.Trim();

        if (trimmedName.Length > ShippingAddressValidationRules.RecipientNameMaxLength)
            throw new ValidationException(
                CustomerShippingAddressErrorCodes.RecipientNameMaxLengthExceeded,
                $"Recipient name cannot exceed {ShippingAddressValidationRules.RecipientNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName",
                    ["max"] = ShippingAddressValidationRules.RecipientNameMaxLength
                }
            );

        RecipientName = trimmedName;
    }

    public void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientPhoneNumber))
            throw new ValidationException(
                CustomerShippingAddressErrorCodes.RecipientPhoneNumberRequired,
                "Recipient phone number is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber"
                }
            );

        var trimmedRecipientPhoneNumber = recipientPhoneNumber.Trim();

        if (trimmedRecipientPhoneNumber.Length > CommonValidationRules.PhoneNumberMaxLength)
            throw new ValidationException(
                CustomerShippingAddressErrorCodes.RecipientPhoneNumberMaxLengthExceeded,
                $"Recipient phone number cannot exceed {CommonValidationRules.PhoneNumberMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber",
                    ["max"] = CommonValidationRules.PhoneNumberMaxLength
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
                CustomerShippingAddressErrorCodes.AlreadyDefault,
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
                CustomerShippingAddressErrorCodes.NotDefault,
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