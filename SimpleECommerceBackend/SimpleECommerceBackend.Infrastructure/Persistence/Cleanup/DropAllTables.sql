IF OBJECT_ID(N'translation.Translations', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Translations')
        ALTER TABLE [translation].[Translations] DROP CONSTRAINT [FK_Translations];
END
GO

IF OBJECT_ID(N'business.CartItems', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CartItems_Products_ProductId')
    ALTER TABLE [business].[CartItems] DROP CONSTRAINT [FK_CartItems_Products_ProductId];
GO

IF OBJECT_ID(N'business.CartItems', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CartItems_Carts_CartId')
    ALTER TABLE [business].[CartItems] DROP CONSTRAINT [FK_CartItems_Carts_CartId];
GO

IF OBJECT_ID(N'business.CustomerShippingAddresses', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CustomerShippingAddresses_UserProfiles_CustomerId')
    ALTER TABLE [business].[CustomerShippingAddresses] DROP CONSTRAINT [FK_CustomerShippingAddresses_UserProfiles_CustomerId];
GO

IF OBJECT_ID(N'business.Inventories', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Inventories_Products_ProductId')
    ALTER TABLE [business].[Inventories] DROP CONSTRAINT [FK_Inventories_Products_ProductId];
GO

IF OBJECT_ID(N'business.Inventories', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Inventories_SellerWarehouses_SellerWarehouseId')
    ALTER TABLE [business].[Inventories] DROP CONSTRAINT [FK_Inventories_SellerWarehouses_SellerWarehouseId];
GO

IF OBJECT_ID(N'business.Notifications', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Notifications_UserProfiles_UserId')
    ALTER TABLE [business].[Notifications] DROP CONSTRAINT [FK_Notifications_UserProfiles_UserId];
GO

IF OBJECT_ID(N'business.OrderItems', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_OrderItems_Orders_OrderId')
    ALTER TABLE [business].[OrderItems] DROP CONSTRAINT [FK_OrderItems_Orders_OrderId];
GO

IF OBJECT_ID(N'business.OrderItems', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_OrderItems_Products_ProductId')
    ALTER TABLE [business].[OrderItems] DROP CONSTRAINT [FK_OrderItems_Products_ProductId];
GO

IF OBJECT_ID(N'business.Orders', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Orders_UserProfiles_CustomerId')
    ALTER TABLE [business].[Orders] DROP CONSTRAINT [FK_Orders_UserProfiles_CustomerId];
GO

IF OBJECT_ID(N'business.Orders', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Orders_UserProfiles_SellerId')
    ALTER TABLE [business].[Orders] DROP CONSTRAINT [FK_Orders_UserProfiles_SellerId];
GO

IF OBJECT_ID(N'business.Payments', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Payments_Orders_OrderId')
    ALTER TABLE [business].[Payments] DROP CONSTRAINT [FK_Payments_Orders_OrderId];
GO

IF OBJECT_ID(N'business.ProductImages', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ProductImages_Products_ProductId')
    ALTER TABLE [business].[ProductImages] DROP CONSTRAINT [FK_ProductImages_Products_ProductId];
GO

IF OBJECT_ID(N'business.ProductPrices', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ProductPrices_Products_ProductId')
    ALTER TABLE [business].[ProductPrices] DROP CONSTRAINT [FK_ProductPrices_Products_ProductId];
GO

IF OBJECT_ID(N'business.Products', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Products_Categories_CategoryId')
    ALTER TABLE [business].[Products] DROP CONSTRAINT [FK_Products_Categories_CategoryId];
GO

IF OBJECT_ID(N'business.Products', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Products_UserProfiles_SellerId')
    ALTER TABLE [business].[Products] DROP CONSTRAINT [FK_Products_UserProfiles_SellerId];
GO

IF OBJECT_ID(N'business.Carts', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Carts_UserProfiles_CustomerId')
    ALTER TABLE [business].[Carts] DROP CONSTRAINT [FK_Carts_UserProfiles_CustomerId];
GO

IF OBJECT_ID(N'business.Categories', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Categories_UserProfiles_AdminId')
    ALTER TABLE [business].[Categories] DROP CONSTRAINT [FK_Categories_UserProfiles_AdminId];
GO

IF OBJECT_ID(N'business.SellerWarehouses', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SellerWarehouses_SellerShops_SellerShopId')
    ALTER TABLE [business].[SellerWarehouses] DROP CONSTRAINT [FK_SellerWarehouses_SellerShops_SellerShopId];
GO

IF OBJECT_ID(N'business.SellerShops', N'U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SellerShops_UserProfiles_SellerId')
    ALTER TABLE [business].[SellerShops] DROP CONSTRAINT [FK_SellerShops_UserProfiles_SellerId];
GO

DROP TABLE IF EXISTS [business].[CartItems];
DROP TABLE IF EXISTS [business].[CustomerShippingAddresses];
DROP TABLE IF EXISTS [business].[Inventories];
DROP TABLE IF EXISTS [business].[Notifications];
DROP TABLE IF EXISTS [business].[OrderItems];
DROP TABLE IF EXISTS [business].[Payments];
DROP TABLE IF EXISTS [business].[ProductImages];
DROP TABLE IF EXISTS [business].[ProductPrices];
DROP TABLE IF EXISTS [business].[Products];
DROP TABLE IF EXISTS [business].[Carts];
DROP TABLE IF EXISTS [business].[Orders];
DROP TABLE IF EXISTS [business].[Categories];
DROP TABLE IF EXISTS [business].[SellerWarehouses];
DROP TABLE IF EXISTS [business].[SellerShops];
DROP TABLE IF EXISTS [business].[UserProfiles];
DROP TABLE IF EXISTS [translation].[Translations];