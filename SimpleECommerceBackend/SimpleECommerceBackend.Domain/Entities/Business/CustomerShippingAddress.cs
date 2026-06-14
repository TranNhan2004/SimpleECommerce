using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class CustomerShippingAddress : EntityBase
{
    public CustomerShippingAddress()
    {
    }

    // private CustomerShippingAddress(
    //     string recipientName,
    //     string recipientPhoneNumber,
    //     Address recipientAddress,
    //     bool isDefault
    // )
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     RecipientName = recipientName;
    //     RecipientPhoneNumber = recipientPhoneNumber;
    //     RecipientAddress = recipientAddress;
    //     IsDefault = isDefault;
    // }

    private string _recipientName = null!;
    private string _recipientPhoneNumber = null!;
    private Address _recipientAddress;
    private bool _isDefault;

    public string RecipientName
    {
        get => _recipientName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    CustomerShippingAddressErrorCodes.RecipientNameRequired,
                    "Recipient name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "RecipientName"
                    }
                );

            var trimmedName = value.Trim();

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

            _recipientName = trimmedName;
        }
    }

    public string RecipientPhoneNumber
    {
        get => _recipientPhoneNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    CustomerShippingAddressErrorCodes.RecipientPhoneNumberRequired,
                    "Recipient phone number is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "RecipientPhoneNumber"
                    }
                );

            var trimmedRecipientPhoneNumber = value.Trim();

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

            _recipientPhoneNumber = trimmedRecipientPhoneNumber;
        }
    }

    public Address RecipientAddress
    {
        get => _recipientAddress;
        set => _recipientAddress = value;
    }

    public bool IsDefault
    {
        get => _isDefault;
        set => _isDefault = value;
    }

    public Guid CustomerId { get; private set; }
    public User? Customer { get; private set; }

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

        IsDefault = true;
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

        IsDefault = false;
    }
}
