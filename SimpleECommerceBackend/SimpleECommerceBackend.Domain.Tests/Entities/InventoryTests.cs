using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class InventoryTests
{
    [Fact]
    public void Create_ShouldCreateInventory_WhenInputIsValid()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), Guid.NewGuid(), 10);

        inventory.QuantityInStock.Should().Be(10);
        inventory.QuantityReserved.Should().Be(0);
        inventory.AvailableQuantity.Should().Be(10);
        inventory.Version.Should().Be(1);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenProductIdIsEmpty()
    {
        var action = () => Inventory.Create(Guid.Empty, Guid.NewGuid(), 10);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(InventoryErrorCodes.ProductRequired);
    }

    [Fact]
    public void SetQuantityOnHand_ShouldThrowValidationException_WhenQuantityExceedsMaximum()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), Guid.NewGuid(), 10);
        var action = () => inventory.QuantityInStock = InventoryValidationRules.MaxQuantity + 1;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(InventoryErrorCodes.QuantityOnHandCannotExceed);
    }

    [Fact]
    public void ReserveStock_ShouldIncreaseReservedQuantity_WhenAvailableStockIsSufficient()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), Guid.NewGuid(), 10);

        inventory.ReserveStock(4);

        inventory.QuantityReserved.Should().Be(4);
        inventory.AvailableQuantity.Should().Be(6);
    }

    [Fact]
    public void ReleaseStock_ShouldThrowValidationException_WhenQuantityExceedsReserved()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), Guid.NewGuid(), 10);
        inventory.ReserveStock(3);
        var action = () => inventory.ReleaseStock(4);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(InventoryErrorCodes.QuantityToReleaseCannotExceedReserved);
    }

    [Fact]
    public void IncreaseVersion_ShouldIncrementVersion()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), Guid.NewGuid(), 10);

        inventory.IncreaseVersion();

        inventory.Version.Should().Be(2);
    }
}
