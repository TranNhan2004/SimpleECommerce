using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class PaymentTests
{
    [Fact]
    public void Create_ShouldCreatePayment_WhenInputIsValid()
    {
        var payment = CreatePayment(PaymentMethod.BankTransfer, "  VNPAY  ");

        payment.Method.Should().Be(PaymentMethod.BankTransfer);
        payment.Provider.Should().Be("VNPAY");
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenOrderIdIsEmpty()
    {
        var action = () => new Payment
        {
            OrderId = Guid.Empty,
            Money = EntityTestData.CreateMoney(),
            Method = PaymentMethod.Cash,
            Status = PaymentStatus.Pending
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(PaymentErrorCodes.OrderIdRequired);
    }

    [Fact]
    public void SetProvider_ShouldThrowValidationException_WhenProviderExceedsMaxLength()
    {
        var payment = CreatePayment(PaymentMethod.Cash);
        var provider = new string('a', PaymentValidationRules.ProviderMaxLength + 1);
        var action = () => payment.Provider = provider;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(PaymentErrorCodes.ProviderMaxLengthExceeded);
    }

    [Fact]
    public void Complete_ShouldSetStatusAndExternalTransactionId_WhenPaymentIsPending()
    {
        var payment = CreatePayment(PaymentMethod.CreditCard);

        payment.Complete("  txn-123  ");

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.ExternalTransactionId.Should().Be("txn-123");
    }

    [Fact]
    public void Fail_ShouldThrowValidationException_WhenPaymentIsNotPending()
    {
        var payment = CreatePayment(PaymentMethod.CreditCard);
        payment.Complete();
        var action = () => payment.Fail();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(PaymentErrorCodes.FailNotAllowed);
    }

    [Fact]
    public void Refund_ShouldSetStatus_WhenPaymentIsCompleted()
    {
        var payment = CreatePayment(PaymentMethod.CreditCard);
        payment.Complete();

        payment.Refund();

        payment.Status.Should().Be(PaymentStatus.Refunded);
    }

    private static Payment CreatePayment(PaymentMethod method, string? provider = null)
    {
        return new Payment
        {
            OrderId = Guid.NewGuid(),
            Money = EntityTestData.CreateMoney(),
            Method = method,
            Provider = provider,
            Status = PaymentStatus.Pending
        };
    }
}
