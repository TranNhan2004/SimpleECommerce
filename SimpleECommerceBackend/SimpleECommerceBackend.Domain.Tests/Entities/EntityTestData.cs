using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

internal static class EntityTestData
{
    public static Address CreateAddress(
        string addressLine = "123 Nguyen Trai",
        string ward = "Ward 1",
        string province = "Ho Chi Minh")
    {
        return new Address(addressLine, ward, province);
    }

    public static Money CreateMoney(decimal amount = 100_000m, string currency = "VND")
    {
        return new Money(amount, currency);
    }

    public static DateOnly CreateBirthDate(int age = 20)
    {
        return DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-age);
    }

    public static ProductVariantImage CreateProductVariantImage(
        string imageUrl = "https://example.com/image.jpg",
        int displayOrder = 1,
        bool isDisplayed = true,
        string? description = "Front view")
    {
        return new ProductVariantImage
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder,
            IsDisplayed = isDisplayed,
            Description = description
        };
    }

    public static SellerWarehouse CreateSellerWarehouse(Guid? sellerShopId = null)
    {
        return new SellerWarehouse
        {
            Id = Guid.NewGuid(),
            FullAddress = CreateAddress(),
            SellerShopId = sellerShopId ?? Guid.NewGuid()
        };
    }

    public static void AssignId(EntityBase entity, Guid id)
    {
        entity.Id = id;
    }
}
