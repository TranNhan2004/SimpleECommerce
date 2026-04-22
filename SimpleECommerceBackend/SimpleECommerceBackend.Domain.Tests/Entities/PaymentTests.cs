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
        var payment = Payment.Create(Guid.NewGuid(), EntityTestData.CreateMoney(), PaymentMethod.BankTransfer, "  VNPAY  ");

        payment.Method.Should().Be(PaymentMethod.BankTransfer);
        payment.Provider.Should().Be("VNPAY");
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenOrderIdIsEmpty()
    {
        var action = () => Payment.Create(Guid.Empty, EntityTestData.CreateMoney(), PaymentMethod.Cash);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(PaymentErrorCodes.OrderIdRequired);
    }

    [Fact]
    public void SetProvider_ShouldThrowValidationException_WhenProviderExceedsMaxLength()
    {
        var payment = Payment.Create(Guid.NewGuid(), EntityTestData.CreateMoney(), PaymentMethod.Cash);
        var provider = new string('a', PaymentValidationRules.ProviderMaxLength + 1);
        var action = () => payment.SetProvider(provider);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(PaymentErrorCodes.ProviderMaxLengthExceeded);
    }

    [Fact]
    public void Complete_ShouldSetStatusAndExternalTransactionId_WhenPaymentIsPending()
    {
        var payment = Payment.Create(Guid.NewGuid(), EntityTestData.CreateMoney(), PaymentMethod.CreditCard);

        payment.Complete("  txn-123  ");

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.ExternalTransactionId.Should().Be("txn-123");
    }

    [Fact]
    public void Fail_ShouldThrowValidationException_WhenPaymentIsNotPending()
    {
        var payment = Payment.Create(Guid.NewGuid(), EntityTestData.CreateMoney(), PaymentMethod.CreditCard);
        payment.Complete();
        var action = () => payment.Fail();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(PaymentErrorCodes.FailNotAllowed);
    }

    [Fact]
    public void Refund_ShouldSetStatus_WhenPaymentIsCompleted()
    {
        var payment = Payment.Create(Guid.NewGuid(), EntityTestData.CreateMoney(), PaymentMethod.CreditCard);
        payment.Complete();

        payment.Refund();

        payment.Status.Should().Be(PaymentStatus.Refunded);
    }
}
