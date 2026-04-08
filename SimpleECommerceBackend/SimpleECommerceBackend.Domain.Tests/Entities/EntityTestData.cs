using System.Reflection;
using FluentAssertions;
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

    public static ProductImage CreateProductImage(
        string imageUrl = "https://example.com/image.jpg",
        int displayOrder = 1,
        bool isDisplayed = true,
        string? description = "Front view")
    {
        return ProductImage.Create(imageUrl, displayOrder, isDisplayed, description);
    }

    public static SellerWarehouse CreateSellerWarehouse(Guid? sellerShopId = null)
    {
        return SellerWarehouse.Create(CreateAddress(), sellerShopId ?? Guid.NewGuid());
    }

    public static void AssignId(Entity entity, Guid id)
    {
        var setIdMethod = typeof(Entity).GetMethod("SetId", BindingFlags.Instance | BindingFlags.NonPublic);
        setIdMethod.Should().NotBeNull();
        setIdMethod!.Invoke(entity, [id]);
    }
}
