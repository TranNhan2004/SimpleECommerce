using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductPrice
{
    public Money Money { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }
}