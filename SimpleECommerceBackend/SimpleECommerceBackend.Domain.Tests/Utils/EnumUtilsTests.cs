using FluentAssertions;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Tests.Utils;

public class EnumUtilsTests
{
    [Theory]
    [InlineData("CreditCard", PaymentMethod.CreditCard)]
    [InlineData("creditcard", PaymentMethod.CreditCard)]
    [InlineData("credit-card", PaymentMethod.CreditCard)]
    [InlineData("credit_card", PaymentMethod.CreditCard)]
    [InlineData("credit card", PaymentMethod.CreditCard)]
    [InlineData("bank-transfer", PaymentMethod.BankTransfer)]
    public void TryParse_ShouldSupportNormalizedNames_WhenSelectorIsProvided(string input, PaymentMethod expected)
    {
        var result = EnumUtils.TryParse(input, out PaymentMethod value, ToApiName);

        result.Should().BeTrue();
        value.Should().Be(expected);
    }

    [Fact]
    public void Parse_ShouldThrowValidationException_WhenInputIsInvalid()
    {
        var action = () => EnumUtils.Parse<PaymentMethod>("crypto", "PaymentMethod", "Test_InvalidEnum", ToApiName);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be("Test_InvalidEnum");
    }

    [Fact]
    public void ToName_ShouldReturnCustomName_WhenEnumValueIsSupported()
    {
        var result = EnumUtils.ToName(
            PaymentMethod.BankTransfer,
            "PaymentMethod",
            "Test_UnsupportedEnum",
            ToApiName);

        result.Should().Be("bank-transfer");
    }

    [Fact]
    public void ToName_ShouldThrowValidationException_WhenEnumValueIsUnsupported()
    {
        var action = () => EnumUtils.ToName(
            (PaymentMethod)999,
            "PaymentMethod",
            "Test_UnsupportedEnum",
            ToApiName);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be("Test_UnsupportedEnum");
    }

    private static string ToApiName(PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethod.CreditCard => "credit-card",
            PaymentMethod.DebitCard => "debit-card",
            PaymentMethod.Cash => "cash",
            PaymentMethod.BankTransfer => "bank-transfer",
            PaymentMethod.EWallet => "e-wallet",
            _ => paymentMethod.ToString()
        };
    }
}
