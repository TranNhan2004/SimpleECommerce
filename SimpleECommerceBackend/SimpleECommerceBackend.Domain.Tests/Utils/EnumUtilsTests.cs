using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Tests.Utils;

public class EnumUtilsTests
{
    [Theory]
    [InlineData("credit-card", PaymentMethod.CreditCard)]
    [InlineData("Credit-Card", PaymentMethod.CreditCard)]
    [InlineData("bank-transfer", PaymentMethod.BankTransfer)]
    public void FromDisplayValue_ShouldReturnEnum_WhenDisplayValueIsValid(string input, PaymentMethod expected)
    {
        var result = EnumUtils.FromDisplayValue<PaymentMethod>(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void FromDisplayValue_ShouldThrowValidationException_WhenInputIsInvalid()
    {
        var action = () => EnumUtils.FromDisplayValue<PaymentMethod>("crypto");

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(EnumConversionErrorCodes.InvalidDisplayValue);
    }

    [Fact]
    public void ToDisplayValue_ShouldReturnDisplayValue_WhenEnumValueIsSupported()
    {
        var result = EnumUtils.ToDisplayValue(PaymentMethod.BankTransfer);

        result.Should().Be("bank-transfer");
    }

    [Fact]
    public void ToDisplayValue_ShouldThrowValidationException_WhenEnumValueIsUnsupported()
    {
        var action = () => EnumUtils.ToDisplayValue((PaymentMethod)999);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(EnumConversionErrorCodes.UnsupportedValue);
    }
}
