using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.ValueObjects;

public sealed record Money
{
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required");

        try
        {
            var parsedCurrency = NodaMoney.Currency.FromCode(
                currency.Trim().ToUpperInvariant()
            );
            Amount = amount;
            Currency = parsedCurrency.Code;
        }
        catch (Exception)
        {
            throw new DomainException($"Currency '{currency}' is not supported");
        }
    }

    public decimal Amount { get; }
    public string Currency { get; }
}