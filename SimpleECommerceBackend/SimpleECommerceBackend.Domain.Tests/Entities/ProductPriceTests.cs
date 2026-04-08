using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class ProductPriceTests
{
    [Fact]
    public void Create_ShouldCreateProductPrice_WhenInputIsValid()
    {
        var effectiveFrom = DateTimeOffset.UtcNow;
        var money = EntityTestData.CreateMoney();

        var productPrice = ProductPrice.Create(money, effectiveFrom);

        productPrice.Money.Should().Be(money);
        productPrice.EffectiveFrom.Should().Be(effectiveFrom);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenPriceAmountIsZero()
    {
        var action = () => ProductPrice.Create(EntityTestData.CreateMoney(0), DateTimeOffset.UtcNow);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductPriceErrorCode.PriceMustBePositive);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenPriceAmountIsNegative()
    {
        var action = () => ProductPrice.Create(new SimpleECommerceBackend.Domain.ValueObjects.Money(-1, "VND"), DateTimeOffset.UtcNow);

        action.Should().Throw<ValidationException>();
    }
}
