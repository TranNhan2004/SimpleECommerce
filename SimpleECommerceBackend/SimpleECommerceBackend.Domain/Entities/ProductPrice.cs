using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities;

public class ProductPrice : Entity, ICreatedTrackable
{
    private ProductPrice()
    {
    }

    private ProductPrice(Money money, DateTimeOffset effectiveFrom)
    {
        SetId(Guid.NewGuid());
        SetMoney(money);
        SetEffectiveFrom(effectiveFrom);
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public Money Money { get; private set; }
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static ProductPrice Create(Money money, DateTimeOffset effectiveFrom)
    {
        return new ProductPrice(money, effectiveFrom);
    }

    private void SetMoney(Money money)
    {
        if (money.Amount <= 0)
            throw new BusinessException("Price amount must be positive");

        Money = money;
    }

    private void SetEffectiveFrom(DateTimeOffset effectiveFrom)
    {
        EffectiveFrom = effectiveFrom;
    }
}