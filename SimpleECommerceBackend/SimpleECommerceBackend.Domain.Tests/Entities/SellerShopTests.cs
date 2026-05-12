using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class SellerShopTests
{
    [Fact]
    public void Create_ShouldCreateSellerShop_WhenInputIsValid()
    {
        var sellerShop = CreateSellerShop();

        sellerShop.Name.Should().Be("My Shop");
        sellerShop.PhoneNumber.Should().Be("0987654321");
        sellerShop.AvatarUrl.Should().Be("avatar.png");
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenSellerIdIsEmpty()
    {
        var action = () => new SellerShop
        {
            SellerId = Guid.Empty,
            Name = "My Shop",
            PhoneNumber = "0987654321",
            AvatarUrl = "avatar.png"
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(SellerShopErrorCodes.SellerRequired);
    }

    [Fact]
    public void SetPhoneNumber_ShouldThrowValidationException_WhenPhoneNumberExceedsMaxLength()
    {
        var sellerShop = CreateSellerShop();
        var phoneNumber = new string('1', CommonValidationRules.PhoneNumberMaxLength + 1);
        var action = () => sellerShop.PhoneNumber = phoneNumber;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(SellerShopErrorCodes.PhoneNumberMaxLengthExceeded);
    }

    [Fact]
    public void AddSellerWarehouse_ShouldAppendWarehouse()
    {
        var sellerShop = CreateSellerShop();
        var warehouse = EntityTestData.CreateSellerWarehouse();

        sellerShop.AddSellerWarehouse(warehouse);

        sellerShop.SellerWarehouses.Should().ContainSingle().Which.Should().Be(warehouse);
    }

    [Fact]
    public void ChangeSellerWarehouse_ShouldUpdateExistingWarehouse_WhenWarehouseExists()
    {
        var sellerShop = CreateSellerShop();
        var warehouseId = Guid.NewGuid();
        var existingWarehouse = EntityTestData.CreateSellerWarehouse();
        var updatedWarehouse = new SellerWarehouse
        {
            Id = Guid.NewGuid(),
            FullAddress = EntityTestData.CreateAddress("999 New Street", "Ward 5", "Hue"),
            SellerShopId = Guid.NewGuid()
        };
        EntityTestData.AssignId(existingWarehouse, warehouseId);
        EntityTestData.AssignId(updatedWarehouse, warehouseId);
        sellerShop.AddSellerWarehouse(existingWarehouse);

        sellerShop.ChangeSellerWarehouse(updatedWarehouse);

        existingWarehouse.FullAddress.Should().Be(EntityTestData.CreateAddress("999 New Street", "Ward 5", "Hue"));
    }

    [Fact]
    public void RemoveSellerWarehouse_ShouldSoftDeleteWarehouse_WhenWarehouseExists()
    {
        var sellerShop = CreateSellerShop();
        var warehouse = EntityTestData.CreateSellerWarehouse();
        sellerShop.AddSellerWarehouse(warehouse);

        sellerShop.RemoveSellerWarehouse(warehouse.Id);

        warehouse.IsDeleted.Should().BeTrue();
    }

    private static SellerShop CreateSellerShop()
    {
        return new SellerShop
        {
            SellerId = Guid.NewGuid(),
            Name = "  My Shop  ",
            PhoneNumber = " 0987654321 ",
            AvatarUrl = "avatar.png"
        };
    }
}
