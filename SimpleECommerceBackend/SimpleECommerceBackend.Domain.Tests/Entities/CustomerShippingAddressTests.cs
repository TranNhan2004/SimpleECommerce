using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class CustomerShippingAddressTests
{
    [Fact]
    public void Create_ShouldCreateAddress_WhenInputIsValid()
    {
        var address = CreateAddress();

        address.RecipientName.Should().Be("Nhan");
        address.RecipientPhoneNumber.Should().Be("0987654321");
        address.IsDefault.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenRecipientNameIsBlank()
    {
        var action = () => new CustomerShippingAddress
        {
            RecipientName = " ",
            RecipientPhoneNumber = "0987654321",
            RecipientAddress = EntityTestData.CreateAddress(),
            IsDefault = false
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CustomerShippingAddressErrorCodes.RecipientNameRequired);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenRecipientPhoneNumberExceedsMaxLength()
    {
        var phoneNumber = new string('1', CommonValidationRules.PhoneNumberMaxLength + 1);
        var action = () => new CustomerShippingAddress
        {
            RecipientName = "Nhan",
            RecipientPhoneNumber = phoneNumber,
            RecipientAddress = EntityTestData.CreateAddress(),
            IsDefault = false
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CustomerShippingAddressErrorCodes.RecipientPhoneNumberMaxLengthExceeded);
    }

    [Fact]
    public void MarkAsDefault_ShouldSetIsDefault_WhenAddressIsNotDefault()
    {
        var address = CreateAddress();

        address.MarkAsDefault();

        address.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void RemoveDefault_ShouldThrowValidationException_WhenAddressIsNotDefault()
    {
        var address = CreateAddress();
        var action = () => address.RemoveDefault();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CustomerShippingAddressErrorCodes.NotDefault);
    }

    [Fact]
    public void SoftDelete_ShouldMarkAddressAsDeleted()
    {
        var address = CreateAddress();

        address.SoftDelete();

        address.IsDeleted.Should().BeTrue();
        address.DeletedAt.Should().NotBeNull();
    }

    private static CustomerShippingAddress CreateAddress()
    {
        return new CustomerShippingAddress
        {
            RecipientName = "  Nhan  ",
            RecipientPhoneNumber = " 0987654321 ",
            RecipientAddress = EntityTestData.CreateAddress(),
            IsDefault = false
        };
    }
}
