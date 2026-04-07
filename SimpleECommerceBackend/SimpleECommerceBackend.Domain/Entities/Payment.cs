using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities;

public class Payment : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private Payment()
    {
    }

    private Payment(
        Guid orderId,
        Money money,
        PaymentMethod method,
        string? provider
    )
    {
        SetId(Guid.NewGuid());
        SetOrderId(orderId);
        SetMoney(money);
        SetMethod(method);
        SetProvider(provider);
        SetStatus(PaymentStatus.Pending);
    }

    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }

    public Money Money { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? Provider { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? ExternalTransactionId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Payment Create(
        Guid orderId,
        Money money,
        PaymentMethod method,
        string? provider = null
    )
    {
        return new Payment(orderId, money, method, provider);
    }

    private void SetOrderId(Guid orderId)
    {
        if (orderId == Guid.Empty)
            throw new ValidationException(
                PaymentErrorCode.OrderIdRequired,
                "Order ID is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "OrderId"
                }
            );

        OrderId = orderId;
    }

    public void SetMoney(Money money)
    {
        Money = money;
    }

    public void SetMethod(PaymentMethod method)
    {
        Method = method;
    }

    public void SetProvider(string? provider)
    {
        if (provider is null)
        {
            Provider = null;
            return;
        }

        var trimmedProvider = provider.Trim();

        if (trimmedProvider.Length > PaymentConstants.ProviderMaxLength)
            throw new ValidationException(
                PaymentErrorCode.ProviderMaxLengthExceeded,
                $"Provider cannot exceed {PaymentConstants.ProviderMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Provider",
                    ["max"] = PaymentConstants.ProviderMaxLength
                }
            );

        Provider = string.IsNullOrWhiteSpace(trimmedProvider) ? null : trimmedProvider;
    }

    private void SetStatus(PaymentStatus status)
    {
        Status = status;
    }

    public void SetExternalTransactionId(string? externalTransactionId)
    {
        if (externalTransactionId is null)
        {
            ExternalTransactionId = null;
            return;
        }

        var trimmedId = externalTransactionId.Trim();

        if (trimmedId.Length > PaymentConstants.ExternalTransactionIdMaxLength)
            throw new ValidationException(
                PaymentErrorCode.ExternalTransactionIdMaxLengthExceeded,
                $"External transaction ID cannot exceed {PaymentConstants.ExternalTransactionIdMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "ExternalTransactionId",
                    ["max"] = PaymentConstants.ExternalTransactionIdMaxLength
                }
            );

        ExternalTransactionId = string.IsNullOrWhiteSpace(trimmedId) ? null : trimmedId;
    }

    public void Complete(string? externalTransactionId = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new ValidationException(
                PaymentErrorCode.CompleteNotAllowed,
                "Only pending payments can be marked as completed",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Complete",
                    ["allowedStates"] = "pending"
                }
            );

        if (externalTransactionId != null)
            SetExternalTransactionId(externalTransactionId);

        SetStatus(PaymentStatus.Completed);
    }

    public void Fail()
    {
        if (Status != PaymentStatus.Pending)
            throw new ValidationException(
                PaymentErrorCode.FailNotAllowed,
                "Only pending payments can be marked as failed",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Fail",
                    ["allowedStates"] = "pending"
                }
            );

        SetStatus(PaymentStatus.Failed);
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new ValidationException(
                PaymentErrorCode.RefundNotAllowed,
                "Only completed payments can be refunded",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Refund",
                    ["allowedStates"] = "completed"
                }
            );

        SetStatus(PaymentStatus.Refunded);
    }
}
