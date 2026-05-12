using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class ProductPriceTests
{
    [Fact]
    public void Create_ShouldCreateProductVariantPrice_WhenInputIsValid()
    {
        var effectiveFrom = DateTimeOffset.UtcNow;
        var money = EntityTestData.CreateMoney();

        var productPrice = new ProductVariantPrice
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Money = money,
            EffectiveFrom = effectiveFrom
        };

        productPrice.Money.Should().Be(money);
        productPrice.EffectiveFrom.Should().Be(effectiveFrom);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenPriceAmountIsZero()
    {
        var action = () => new ProductVariantPrice
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Money = EntityTestData.CreateMoney(0),
            EffectiveFrom = DateTimeOffset.UtcNow
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductVariantPriceErrorCodes.PriceMustBePositive);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenPriceAmountIsNegative()
    {
        var action = () => new ProductVariantPrice
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Money = new SimpleECommerceBackend.Domain.ValueObjects.Money(-1, "VND"),
            EffectiveFrom = DateTimeOffset.UtcNow
        };

        action.Should().Throw<ValidationException>();
    }
}
