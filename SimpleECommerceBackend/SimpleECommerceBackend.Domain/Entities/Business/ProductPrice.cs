using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductPrice : EntityBase, ICreatedTime
{
    private ProductPrice()
    {
    }

    private ProductPrice(Money money, DateTimeOffset effectiveFrom)
    {
        SetMoney(money);
        SetEffectiveFrom(effectiveFrom);
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public Money Money { get; private set; } = new(0, "VND");
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static ProductPrice Create(Money money, DateTimeOffset effectiveFrom)
    {
        return new ProductPrice(money, effectiveFrom);
    }

    private void SetMoney(Money money)
    {
        if (money.Amount <= 0)
            throw new DomainException("Price amount must be positive");

        Money = money;
    }

    private void SetEffectiveFrom(DateTimeOffset effectiveFrom)
    {
        EffectiveFrom = effectiveFrom;
    }
}