namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class MoneyErrorCodes
{
    public const string AmountCannotBeNegative = "Money_AmountCannotBeNegative";
    public const string CurrencyRequired = "Money_CurrencyRequired";
    public const string CurrencyUnsupported = "Money_CurrencyUnsupported";
    public const string CurrencyMismatch = "Money_CurrencyMismatch";
    public const string ResultCannotBeNegative = "Money_ResultCannotBeNegative";
}
