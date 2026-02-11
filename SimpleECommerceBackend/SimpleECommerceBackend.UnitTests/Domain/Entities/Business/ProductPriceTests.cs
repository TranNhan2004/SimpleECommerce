// using FluentAssertions;
// using SimpleECommerceBackend.Domain.Entities.Business;
// using SimpleECommerceBackend.Domain.Exceptions;
// using SimpleECommerceBackend.Domain.ValueObjects;
//
// namespace SimpleECommerceBackend.UnitTests.Domain.Entities.Business;
//
// public class ProductPriceTests
// {
//     // ---------- Happy path ----------
//
//     [Fact]
//     public void Create_ShouldCreateProductPrice_WhenInputIsValid()
//     {
//         // Arrange
//         var productId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var money = new Money(100000, "VND");
//         var effectiveFrom = DateTimeOffset.UtcNow;
//
//         // Act
//         var productPrice = ProductPrice.Create(productId, money, effectiveFrom);
//
//         // Assert
//         productPrice.Should().NotBeNull();
//         productPrice.ProductId.Should().Be(productId);
//         productPrice.Money.Should().Be(money);
//         productPrice.EffectiveFrom.Should().Be(effectiveFrom);
//     }
//
//     // ---------- ProductId validation ----------
//
//     [Fact]
//     public void Create_ShouldThrow_WhenProductIdIsEmpty()
//     {
//         // Arrange
//         var productId = Guid.Empty;
//         var money = new Money(100000, "VND");
//         var effectiveFrom = DateTimeOffset.UtcNow;
//
//         // Act
//         var act = () => ProductPrice.Create(productId, money, effectiveFrom);
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Product ID is required");
//     }
//
//     // ---------- Money validation ----------
//
//     [Fact]
//     public void Create_ShouldThrow_WhenMoneyAmountIsZero()
//     {
//         // Arrange
//         var productId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var money = new Money(0, "VND");
//         var effectiveFrom = DateTimeOffset.UtcNow;
//
//         // Act
//         var act = () => ProductPrice.Create(productId, money, effectiveFrom);
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Price amount must be positive");
//     }
//
//     // ---------- SetMoney method ----------
//
//     [Fact]
//     public void SetMoney_ShouldUpdateMoney_WhenAmountIsPositive()
//     {
//         // Arrange
//         var productId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var initialMoney = new Money(100000, "VND");
//         var effectiveFrom = DateTimeOffset.UtcNow;
//         var productPrice = ProductPrice.Create(productId, initialMoney, effectiveFrom);
//
//         var newMoney = new Money(150000, "VND");
//
//         // Act
//         productPrice.SetMoney(newMoney);
//
//         // Assert
//         productPrice.Money.Should().Be(newMoney);
//     }
//
//     [Fact]
//     public void SetMoney_ShouldThrow_WhenAmountIsNotPositive()
//     {
//         // Arrange
//         var productId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var initialMoney = new Money(100000, "VND");
//         var effectiveFrom = DateTimeOffset.UtcNow;
//         var productPrice = ProductPrice.Create(productId, initialMoney, effectiveFrom);
//
//         var invalidMoney = new Money(0, "VND");
//
//         // Act
//         var act = () => productPrice.SetMoney(invalidMoney);
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Price amount must be positive");
//     }
// }

