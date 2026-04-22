namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class PaymentErrorCodes
{
    public const string OrderIdRequired = "Payment_OrderIdRequired";
    public const string ProviderMaxLengthExceeded = "Payment_ProviderMaxLengthExceeded";
    public const string ExternalTransactionIdMaxLengthExceeded = "Payment_ExternalTransactionIdMaxLengthExceeded";
    public const string CompleteNotAllowed = "Payment_CompleteNotAllowed";
    public const string FailNotAllowed = "Payment_FailNotAllowed";
    public const string RefundNotAllowed = "Payment_RefundNotAllowed";
}
