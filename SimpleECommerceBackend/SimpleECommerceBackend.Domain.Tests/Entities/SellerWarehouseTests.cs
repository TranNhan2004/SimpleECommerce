using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class SellerWarehouseTests
{
    [Fact]
    public void Create_ShouldCreateSellerWarehouse_WhenInputIsValid()
    {
        var address = EntityTestData.CreateAddress();
        var sellerShopId = Guid.NewGuid();

        var warehouse = new SellerWarehouse
        {
            FullAddress = address,
            SellerShopId = sellerShopId
        };

        warehouse.FullAddress.Should().Be(address);
        warehouse.SellerShopId.Should().Be(sellerShopId);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenSellerShopIdIsEmpty()
    {
        var action = () => new SellerWarehouse
        {
            FullAddress = EntityTestData.CreateAddress(),
            SellerShopId = Guid.Empty
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(SellerWarehouseErrorCodes.SellerShopRequired);
    }

    [Fact]
    public void SetFullAddress_ShouldUpdateAddress()
    {
        var warehouse = new SellerWarehouse
        {
            FullAddress = EntityTestData.CreateAddress(),
            SellerShopId = Guid.NewGuid()
        };
        var address = EntityTestData.CreateAddress("99 Tran Hung Dao", "Ward 3", "Can Tho");

        warehouse.FullAddress = address;

        warehouse.FullAddress.Should().Be(address);
    }

    [Fact]
    public void SoftDelete_ShouldThrowValidationException_WhenWarehouseIsAlreadyDeleted()
    {
        var warehouse = new SellerWarehouse
        {
            FullAddress = EntityTestData.CreateAddress(),
            SellerShopId = Guid.NewGuid()
        };
        warehouse.SoftDelete();
        var action = () => warehouse.SoftDelete();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(SellerWarehouseErrorCodes.AlreadyDeleted);
    }
}
