using FluentAssertions;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    // ---------- Happy path ----------

    [Fact]
    public void Constructor_ShouldCreateMoney_WhenInputIsValid()
    {
        // Act
        var money = new Money(100.5m, "USD");

        // Assert
        money.Amount.Should().Be(100.5m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Constructor_ShouldRoundAmount_BasedOnCurrencyMinorUnit()
    {
        // Act
        var vnd = new Money(100.6m, "VND"); // VND MinorUnit = 0
        var usd = new Money(100.555m, "USD"); // USD MinorUnit = 2

        // Assert
        vnd.Amount.Should().Be(101m);
        usd.Amount.Should().Be(100.56m);
    }

    // ---------- Validation ----------

    [Fact]
    public void Constructor_ShouldThrow_WhenAmountIsNegative()
    {
        // Act
        var act = () => new Money(-1, "VND");

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Money amount cannot be negative");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenCurrencyIsEmptyOrWhitespace(string currencyCode)
    {
        // Act
        var act = () => new Money(100, currencyCode);

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Currency is required");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCurrencyIsNotSupported()
    {
        // Act
        var act = () => new Money(100, "INVALID");

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Currency 'INVALID' is not supported");
    }

    // ---------- Operators ----------

    [Fact]
    public void Addition_ShouldReturnSum_WhenCurrenciesMatch()
    {
        // Arrange
        var m1 = new Money(100, "VND");
        var m2 = new Money(200, "VND");

        // Act
        var result = m1 + m2;

        // Assert
        result.Amount.Should().Be(300);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public void Addition_ShouldThrow_WhenCurrenciesMismatch()
    {
        // Arrange
        var m1 = new Money(100, "VND");
        var m2 = new Money(100, "USD");

        // Act
        var act = () =>
        {
            var _ = m1 + m2;
        };

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Currency mismatch: Cannot operate between VND and USD");
    }

    [Fact]
    public void Subtraction_ShouldReturnDifference_WhenCurrenciesMatch()
    {
        // Arrange
        var m1 = new Money(500, "USD");
        var m2 = new Money(200, "USD");

        // Act
        var result = m1 - m2;

        // Assert
        result.Amount.Should().Be(300);
    }

    [Fact]
    public void Subtraction_ShouldThrow_WhenResultIsNegative()
    {
        // Arrange
        var m1 = new Money(100, "VND");
        var m2 = new Money(200, "VND");

        // Act
        var act = () =>
        {
            var _ = m1 - m2;
        };

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Result of subtraction cannot be negative");
    }

    [Fact]
    public void Multiplication_ShouldScaleAmount_WhenMultipliedByFactor()
    {
        // Arrange
        var money = new Money(100, "USD");

        // Act
        var result = money * 2.5m;

        // Assert
        result.Amount.Should().Be(250);
        result.Currency.Should().Be("USD");
    }

    // ---------- Comparison ----------

    [Fact]
    public void Comparison_ShouldWorkCorrectly_ForSameCurrency()
    {
        // Arrange
        var small = new Money(100, "USD");
        var large = new Money(200, "USD");
        var equal = new Money(100, "USD");

        // Assert
        (large > small).Should().BeTrue();
        (small < large).Should().BeTrue();
        (small >= equal).Should().BeTrue();
        (small <= equal).Should().BeTrue();
        (small == equal).Should().BeTrue();
    }

    // ---------- Formatting ----------

    [Theory]
    [InlineData(1000, "VND", "1,000 VND")]
    [InlineData(1000.5, "USD", "1,000.50 USD")]
    public void ToString_ShouldFormatBasedOnCurrency(decimal amount, string code, string expected)
    {
        // Act
        var money = new Money(amount, code);

        // Assert
        // Dùng Replace("\u00A0", " ") nếu Culture của bạn tạo ra non-breaking space
        money.ToString().Should().Be(expected);
    }
}