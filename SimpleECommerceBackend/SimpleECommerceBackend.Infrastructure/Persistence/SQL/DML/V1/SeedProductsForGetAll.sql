/*
    Seed data for testing the customer get-all-products API after the
    Product -> ProductVariant schema split.

    Assumptions:
    1. DDL/V1/InitialCreate.sql has been applied.
    2. DDL/V1/UpdateProduct.sql has been applied if needed.
*/

SET NOCOUNT ON;

DECLARE @AdminId uniqueidentifier = '00000000-0000-0000-0000-000000000001';
DECLARE @SellerId uniqueidentifier = '00000000-0000-0000-0000-000000000002';
DECLARE @CustomerId uniqueidentifier = '00000000-0000-0000-0000-000000000003';

DECLARE @CategoryElectronicsId uniqueidentifier = '10000000-0000-0000-0000-000000000001';
DECLARE @CategoryHomeId uniqueidentifier = '10000000-0000-0000-0000-000000000002';

DECLARE @ProductMouseId uniqueidentifier = '20000000-0000-0000-0000-000000000001';
DECLARE @ProductPlanterId uniqueidentifier = '20000000-0000-0000-0000-000000000002';

DECLARE @VariantMouseBlackId uniqueidentifier = '30000000-0000-0000-0000-000000000001';
DECLARE @VariantMouseWhiteId uniqueidentifier = '30000000-0000-0000-0000-000000000002';
DECLARE @VariantPlanterSmallId uniqueidentifier = '30000000-0000-0000-0000-000000000003';
DECLARE @VariantPlanterLargeId uniqueidentifier = '30000000-0000-0000-0000-000000000004';

DECLARE @WarehouseId uniqueidentifier = '40000000-0000-0000-0000-000000000001';
DECLARE @ShopId uniqueidentifier = '50000000-0000-0000-0000-000000000001';

IF NOT EXISTS (SELECT 1 FROM [uam].[Users] WHERE [Id] = @AdminId)
BEGIN
    INSERT INTO [uam].[Users]
    (
        [Id], [Email], [FirstName], [LastName], [NickName], [Sex], [Status],
        [BirthDate], [AvatarUrl], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (@AdminId, N'admin@example.com', N'System', N'Admin', NULL, 0, 1, '1990-01-01', NULL, SYSDATETIMEOFFSET(), NULL),
    (@SellerId, N'seller@example.com', N'Jane', N'Seller', NULL, 0, 1, '1992-01-01', NULL, SYSDATETIMEOFFSET(), NULL),
    (@CustomerId, N'customer@example.com', N'John', N'Customer', NULL, 0, 1, '1994-01-01', NULL, SYSDATETIMEOFFSET(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM [business].[Categories] WHERE [Id] = @CategoryElectronicsId)
BEGIN
    INSERT INTO [business].[Categories]
    (
        [Id], [Name], [Description], [Status], [AdminId], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (@CategoryElectronicsId, N'Electronics', N'Electronic devices and accessories', 1, @AdminId, SYSDATETIMEOFFSET(), NULL),
    (@CategoryHomeId, N'Home', N'Home and living items', 1, @AdminId, SYSDATETIMEOFFSET(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM [business].[SellerShops] WHERE [Id] = @ShopId)
BEGIN
    INSERT INTO [business].[SellerShops]
    (
        [Id], [SellerId], [Name], [PhoneNumber], [AvatarUrl], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (@ShopId, @SellerId, N'Jane Shop', N'0123456789', NULL, SYSDATETIMEOFFSET(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM [business].[SellerWarehouses] WHERE [Id] = @WarehouseId)
BEGIN
    INSERT INTO [business].[SellerWarehouses]
    (
        [Id], [SellerShopId], [AddressLine], [Province], [Ward], [CreatedAt], [IsDeleted], [DeletedAt], [UpdatedAt]
    )
    VALUES
    (@WarehouseId, @ShopId, N'123 Warehouse Street', N'Ho Chi Minh', N'Ward 1', SYSDATETIMEOFFSET(), 0, NULL, NULL);
END

IF NOT EXISTS (SELECT 1 FROM [business].[Products] WHERE [Id] = @ProductMouseId)
BEGIN
    INSERT INTO [business].[Products]
    (
        [Id], [Name], [Description], [AverageRating], [TotalRatings], [Status], [CategoryId], [SellerId], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (@ProductMouseId, N'Wireless Mouse', N'Comfortable wireless mouse with silent click switches', 4.50, 12, 2, @CategoryElectronicsId, @SellerId, SYSDATETIMEOFFSET(), NULL),
    (@ProductPlanterId, N'Ceramic Planter', N'Ceramic planter for indoor decoration', 4.80, 8, 2, @CategoryHomeId, @SellerId, SYSDATETIMEOFFSET(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM [business].[ProductVariants] WHERE [Id] = @VariantMouseBlackId)
BEGIN
    INSERT INTO [business].[ProductVariants]
    (
        [Id], [ProductId], [Name], [Description], [CurrentAmount], [Currency], [TotalInStock],
        [DefaultImageUrl], [Status], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (@VariantMouseBlackId, @ProductMouseId, N'Black', N'Black wireless mouse variant', 249000, N'VND', 25, N'https://cdn.example.com/products/mouse-black/main.png', 1, SYSDATETIMEOFFSET(), NULL),
    (@VariantMouseWhiteId, @ProductMouseId, N'White', N'White wireless mouse variant', 259000, N'VND', 18, N'https://cdn.example.com/products/mouse-white/main.png', 1, SYSDATETIMEOFFSET(), NULL),
    (@VariantPlanterSmallId, @ProductPlanterId, N'Small', N'Small ceramic planter', 189000, N'VND', 30, N'https://cdn.example.com/products/planter-small/main.png', 1, SYSDATETIMEOFFSET(), NULL),
    (@VariantPlanterLargeId, @ProductPlanterId, N'Large', N'Large ceramic planter', 279000, N'VND', 14, N'https://cdn.example.com/products/planter-large/main.png', 1, SYSDATETIMEOFFSET(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM [business].[ProductVariantPrices] WHERE [ProductVariantId] = @VariantMouseBlackId)
BEGIN
    INSERT INTO [business].[ProductVariantPrices]
    (
        [Id], [ProductVariantId], [EffectiveFrom], [Amount], [Currency], [CreatedAt]
    )
    VALUES
    (NEWID(), @VariantMouseBlackId, SYSDATETIMEOFFSET(), 249000, N'VND', SYSDATETIMEOFFSET()),
    (NEWID(), @VariantMouseWhiteId, SYSDATETIMEOFFSET(), 259000, N'VND', SYSDATETIMEOFFSET()),
    (NEWID(), @VariantPlanterSmallId, SYSDATETIMEOFFSET(), 189000, N'VND', SYSDATETIMEOFFSET()),
    (NEWID(), @VariantPlanterLargeId, SYSDATETIMEOFFSET(), 279000, N'VND', SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [business].[ProductVariantImages] WHERE [ProductVariantId] = @VariantMouseBlackId)
BEGIN
    INSERT INTO [business].[ProductVariantImages]
    (
        [Id], [ProductVariantId], [ImageUrl], [DisplayOrder], [IsDisplayed], [Description], [CreatedAt]
    )
    VALUES
    (NEWID(), @VariantMouseBlackId, N'https://cdn.example.com/products/mouse-black/main.png', 1, 1, N'Primary image', SYSDATETIMEOFFSET()),
    (NEWID(), @VariantMouseWhiteId, N'https://cdn.example.com/products/mouse-white/main.png', 1, 1, N'Primary image', SYSDATETIMEOFFSET()),
    (NEWID(), @VariantPlanterSmallId, N'https://cdn.example.com/products/planter-small/main.png', 1, 1, N'Primary image', SYSDATETIMEOFFSET()),
    (NEWID(), @VariantPlanterLargeId, N'https://cdn.example.com/products/planter-large/main.png', 1, 1, N'Primary image', SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [business].[Inventories] WHERE [ProductVariantId] = @VariantMouseBlackId AND [SellerWarehouseId] = @WarehouseId)
BEGIN
    INSERT INTO [business].[Inventories]
    (
        [Id], [ProductVariantId], [SellerWarehouseId], [QuantityInStock], [QuantityReserved], [Version], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (NEWID(), @VariantMouseBlackId, @WarehouseId, 25, 2, 1, SYSDATETIMEOFFSET(), NULL),
    (NEWID(), @VariantMouseWhiteId, @WarehouseId, 18, 1, 1, SYSDATETIMEOFFSET(), NULL),
    (NEWID(), @VariantPlanterSmallId, @WarehouseId, 30, 3, 1, SYSDATETIMEOFFSET(), NULL),
    (NEWID(), @VariantPlanterLargeId, @WarehouseId, 14, 0, 1, SYSDATETIMEOFFSET(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM [business].[Reviews] WHERE [ProductId] = @ProductMouseId AND [CustomerId] = @CustomerId)
BEGIN
    DECLARE @ReviewId uniqueidentifier = NEWID();

    INSERT INTO [business].[Reviews]
    (
        [Id], [ProductId], [CustomerId], [Rating], [Comment], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (@ReviewId, @ProductMouseId, @CustomerId, 5, N'Great mouse, smooth tracking and quiet clicks.', SYSDATETIMEOFFSET(), NULL);

    INSERT INTO [business].[ReviewResponses]
    (
        [Id], [ReviewId], [FromUserId], [ToUserId], [Comment], [CreatedAt]
    )
    VALUES
    (NEWID(), @ReviewId, @SellerId, @CustomerId, N'Thank you for the feedback.', SYSDATETIMEOFFSET());
END
