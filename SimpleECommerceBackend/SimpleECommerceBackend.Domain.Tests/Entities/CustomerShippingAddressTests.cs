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
        var address = CustomerShippingAddress.Create(
            "  Nhan  ",
            " 0987654321 ",
            EntityTestData.CreateAddress());

        address.RecipientName.Should().Be("Nhan");
        address.RecipientPhoneNumber.Should().Be("0987654321");
        address.IsDefault.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenRecipientNameIsBlank()
    {
        var action = () => CustomerShippingAddress.Create(" ", "0987654321", EntityTestData.CreateAddress());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CustomerShippingAddressErrorCode.RecipientNameRequired);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenRecipientPhoneNumberExceedsMaxLength()
    {
        var phoneNumber = new string('1', CommonConstants.PhoneNumberMaxLength + 1);
        var action = () => CustomerShippingAddress.Create("Nhan", phoneNumber, EntityTestData.CreateAddress());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CustomerShippingAddressErrorCode.RecipientPhoneNumberMaxLengthExceeded);
    }

    [Fact]
    public void MarkAsDefault_ShouldSetIsDefault_WhenAddressIsNotDefault()
    {
        var address = CustomerShippingAddress.Create("Nhan", "0987654321", EntityTestData.CreateAddress());

        address.MarkAsDefault();

        address.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void RemoveDefault_ShouldThrowValidationException_WhenAddressIsNotDefault()
    {
        var address = CustomerShippingAddress.Create("Nhan", "0987654321", EntityTestData.CreateAddress());
        var action = () => address.RemoveDefault();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CustomerShippingAddressErrorCode.NotDefault);
    }

    [Fact]
    public void SoftDelete_ShouldMarkAddressAsDeleted()
    {
        var address = CustomerShippingAddress.Create("Nhan", "0987654321", EntityTestData.CreateAddress());

        address.SoftDelete();

        address.IsDeleted.Should().BeTrue();
        address.DeletedAt.Should().NotBeNull();
    }
}
