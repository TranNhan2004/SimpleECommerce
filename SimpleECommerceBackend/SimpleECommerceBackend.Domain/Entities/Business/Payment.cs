using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Payment : Entity, ICreatedTrackable, IUpdatedTrackable
{
    public Payment()
    {
    }

    // private Payment(
    //     Guid orderId,
    //     Money money,
    //     PaymentMethod method,
    //     string? provider
    // )
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     OrderId = orderId;
    //     Money = money;
    //     Method = method;
    //     Provider = provider;
    //     Status = PaymentStatus.Pending;
    // }

    private Guid _orderId;
    private Money _money;
    private PaymentMethod _method;
    private string? _provider;
    private PaymentStatus _status;
    private string? _externalTransactionId;

    public Guid OrderId
    {
        get => _orderId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    PaymentErrorCodes.OrderIdRequired,
                    "Order ID is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "OrderId"
                    }
                );

            _orderId = value;
        }
    }

    public Order? Order { get; private set; }

    public Money Money
    {
        get => _money;
        set => _money = value;
    }

    public PaymentMethod Method
    {
        get => _method;
        set => _method = value;
    }

    public string? Provider
    {
        get => _provider;
        set
        {
            if (value is null)
            {
                _provider = null;
                return;
            }

            var trimmedProvider = value.Trim();

            if (trimmedProvider.Length > PaymentValidationRules.ProviderMaxLength)
                throw new ValidationException(
                    PaymentErrorCodes.ProviderMaxLengthExceeded,
                    $"Provider cannot exceed {PaymentValidationRules.ProviderMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Provider",
                        ["max"] = PaymentValidationRules.ProviderMaxLength
                    }
                );

            _provider = string.IsNullOrWhiteSpace(trimmedProvider) ? null : trimmedProvider;
        }
    }

    public PaymentStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public string? ExternalTransactionId
    {
        get => _externalTransactionId;
        set
        {
            if (value is null)
            {
                _externalTransactionId = null;
                return;
            }

            var trimmedId = value.Trim();

            if (trimmedId.Length > PaymentValidationRules.ExternalTransactionIdMaxLength)
                throw new ValidationException(
                    PaymentErrorCodes.ExternalTransactionIdMaxLengthExceeded,
                    $"External transaction ID cannot exceed {PaymentValidationRules.ExternalTransactionIdMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ExternalTransactionId",
                        ["max"] = PaymentValidationRules.ExternalTransactionIdMaxLength
                    }
                );

            _externalTransactionId = string.IsNullOrWhiteSpace(trimmedId) ? null : trimmedId;
        }
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public void Complete(string? externalTransactionId = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new ValidationException(
                PaymentErrorCodes.CompleteNotAllowed,
                "Only pending payments can be marked as completed",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Complete",
                    ["allowedStates"] = "pending"
                }
            );

        if (externalTransactionId != null)
            ExternalTransactionId = externalTransactionId;

        Status = PaymentStatus.Completed;
    }

    public void Fail()
    {
        if (Status != PaymentStatus.Pending)
            throw new ValidationException(
                PaymentErrorCodes.FailNotAllowed,
                "Only pending payments can be marked as failed",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Fail",
                    ["allowedStates"] = "pending"
                }
            );

        Status = PaymentStatus.Failed;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new ValidationException(
                PaymentErrorCodes.RefundNotAllowed,
                "Only completed payments can be refunded",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Refund",
                    ["allowedStates"] = "completed"
                }
            );

        Status = PaymentStatus.Refunded;
    }
}
