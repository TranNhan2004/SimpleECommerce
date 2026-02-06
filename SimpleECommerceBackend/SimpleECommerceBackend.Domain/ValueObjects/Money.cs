using NodaMoney;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.ValueObjects;

public readonly record struct Money
{
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required");

        try
        {
            var nodaMoney = new NodaMoney.Money(amount, currency.Trim().ToUpperInvariant());

            Amount = nodaMoney.Amount;
            Currency = nodaMoney.Currency.Code;
        }
        catch (Exception)
        {
            throw new DomainException($"Currency '{currency}' is not supported");
        }
    }

    public decimal Amount { get; }
    public string Currency { get; }


    private static void CheckSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException($"Currency mismatch: Cannot operate between {a.Currency} and {b.Currency}");
    }

    public override string ToString()
    {
        var info = CurrencyInfo.FromCode(Currency);
        var decimals = (int)info.MinorUnit;

        return $"{Amount.ToString("N" + decimals)} {Currency}";
    }

    public static Money operator +(Money a, Money b)
    {
        CheckSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        CheckSameCurrency(a, b);
        var result = a.Amount - b.Amount;
        if (result < 0) throw new DomainException("Result of subtraction cannot be negative");
        return new Money(result, a.Currency);
    }

    public static Money operator *(Money m, decimal factor)
    {
        return new Money(m.Amount * factor, m.Currency);
    }

    public static Money operator *(decimal factor, Money m)
    {
        return m * factor;
    }

    public static bool operator >(Money a, Money b)
    {
        CheckSameCurrency(a, b);
        return a.Amount > b.Amount;
    }

    public static bool operator <(Money a, Money b)
    {
        CheckSameCurrency(a, b);
        return a.Amount < b.Amount;
    }

    public static bool operator >=(Money a, Money b)
    {
        return a > b || a == b;
    }

    public static bool operator <=(Money a, Money b)
    {
        return a < b || a == b;
    }
}