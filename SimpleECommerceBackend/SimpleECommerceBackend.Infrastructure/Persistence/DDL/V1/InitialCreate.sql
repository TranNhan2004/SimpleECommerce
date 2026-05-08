IF SCHEMA_ID(N'business') IS NULL
    EXEC(N'CREATE SCHEMA business');
GO

IF SCHEMA_ID(N'translation') IS NULL
    EXEC(N'CREATE SCHEMA translation');
GO

CREATE TABLE [business].[UserProfiles]
(
    [Id] uniqueidentifier NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(64) NOT NULL,
    [LastName] nvarchar(128) NOT NULL,
    [NickName] nvarchar(128) NULL,
    [Sex] int NOT NULL,
    [Status] int NOT NULL,
    [BirthDate] date NOT NULL,
    [AvatarUrl] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_UserProfiles] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_UserProfiles_Email]
    ON [business].[UserProfiles] ([Email])
    WHERE [Status] <> 99;
GO

CREATE TABLE [business].[Categories]
(
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(1024) NULL,
    [Status] int NOT NULL,
    [AdminId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_UserProfiles_AdminId] FOREIGN KEY ([AdminId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Categories_AdminId]
    ON [business].[Categories] ([AdminId]);
GO

CREATE TABLE [business].[SellerShops]
(
    [Id] uniqueidentifier NOT NULL,
    [SellerId] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(10) NOT NULL,
    [AvatarUrl] nvarchar(max) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_SellerShops] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SellerShops_UserProfiles_SellerId] FOREIGN KEY ([SellerId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_SellerShops_SellerId]
    ON [business].[SellerShops] ([SellerId]);
GO

CREATE TABLE [business].[SellerWarehouses]
(
    [Id] uniqueidentifier NOT NULL,
    [SellerShopId] uniqueidentifier NOT NULL,
    [AddressLine] nvarchar(256) NOT NULL,
    [Province] nvarchar(max) NOT NULL,
    [Ward] nvarchar(max) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [IsDeleted] bit NOT NULL CONSTRAINT [DF_SellerWarehouses_IsDeleted] DEFAULT (0),
    [DeletedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_SellerWarehouses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SellerWarehouses_SellerShops_SellerShopId] FOREIGN KEY ([SellerShopId])
        REFERENCES [business].[SellerShops] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_SellerWarehouses_SellerShopId]
    ON [business].[SellerWarehouses] ([SellerShopId]);
GO

CREATE TABLE [business].[Products]
(
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(512) NOT NULL,
    [Description] nvarchar(2048) NOT NULL,
    [CurrentAmount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [TotalInStock] int NOT NULL,
    [Status] int NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [SellerId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId])
        REFERENCES [business].[Categories] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Products_UserProfiles_SellerId] FOREIGN KEY ([SellerId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Products_CategoryId]
    ON [business].[Products] ([CategoryId]);
GO

CREATE INDEX [IX_Products_SellerId]
    ON [business].[Products] ([SellerId]);
GO

CREATE TABLE [business].[ProductImages]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsDisplayed] bit NOT NULL CONSTRAINT [DF_ProductImages_IsDisplayed] DEFAULT (1),
    [Description] nvarchar(512) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [business].[Products] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ProductImages_ProductId]
    ON [business].[ProductImages] ([ProductId]);
GO

CREATE TABLE [business].[ProductPrices]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [EffectiveFrom] datetimeoffset NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_ProductPrices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductPrices_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [business].[Products] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ProductPrices_ProductId]
    ON [business].[ProductPrices] ([ProductId]);
GO

CREATE TABLE [business].[Carts]
(
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Carts_UserProfiles_CustomerId] FOREIGN KEY ([CustomerId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Carts_CustomerId]
    ON [business].[Carts] ([CustomerId]);
GO

CREATE TABLE [business].[CartItems]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [CartId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [business].[Products] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId])
        REFERENCES [business].[Carts] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CartItems_CartId]
    ON [business].[CartItems] ([CartId]);
GO

CREATE INDEX [IX_CartItems_ProductId]
    ON [business].[CartItems] ([ProductId]);
GO

CREATE TABLE [business].[CustomerShippingAddresses]
(
    [Id] uniqueidentifier NOT NULL,
    [RecipientName] nvarchar(256) NOT NULL,
    [RecipientPhoneNumber] nvarchar(10) NOT NULL,
    [RecipientAddressLine] nvarchar(256) NOT NULL,
    [RecipientProvince] nvarchar(max) NOT NULL,
    [RecipientWard] nvarchar(max) NOT NULL,
    [IsDefault] bit NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [IsDeleted] bit NOT NULL CONSTRAINT [DF_CustomerShippingAddresses_IsDeleted] DEFAULT (0),
    [DeletedAt] datetimeoffset NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_CustomerShippingAddresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CustomerShippingAddresses_UserProfiles_CustomerId] FOREIGN KEY ([CustomerId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CustomerShippingAddresses_CustomerId]
    ON [business].[CustomerShippingAddresses] ([CustomerId]);
GO

CREATE TABLE [business].[Inventories]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [SellerWarehouseId] uniqueidentifier NOT NULL,
    [QuantityInStock] int NOT NULL,
    [QuantityReserved] int NOT NULL,
    [Version] int NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Inventories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Inventories_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [business].[Products] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_Inventories_SellerWarehouses_SellerWarehouseId] FOREIGN KEY ([SellerWarehouseId])
        REFERENCES [business].[SellerWarehouses] ([Id])
        ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Inventories_ProductId_SellerWarehouseId]
    ON [business].[Inventories] ([ProductId], [SellerWarehouseId]);
GO

CREATE INDEX [IX_Inventories_SellerWarehouseId]
    ON [business].[Inventories] ([SellerWarehouseId]);
GO

CREATE TABLE [business].[Notifications]
(
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Message] nvarchar(2048) NOT NULL,
    [IsRead] bit NOT NULL CONSTRAINT [DF_Notifications_IsRead] DEFAULT (0),
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_UserProfiles_UserId] FOREIGN KEY ([UserId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Notifications_CreatedAt]
    ON [business].[Notifications] ([CreatedAt]);
GO

CREATE INDEX [IX_Notifications_UserId]
    ON [business].[Notifications] ([UserId]);
GO

CREATE TABLE [business].[Orders]
(
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(64) NOT NULL,
    [Note] nvarchar(1024) NULL,
    [ShippingAmount] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [TotalCurrency] nvarchar(3) NOT NULL,
    [Status] int NOT NULL,
    [ShopName] nvarchar(256) NOT NULL,
    [WarehouseAddressLine] nvarchar(256) NOT NULL,
    [WarehouseProvince] nvarchar(max) NOT NULL,
    [WarehouseWard] nvarchar(max) NOT NULL,
    [RecipientName] nvarchar(256) NOT NULL,
    [RecipientPhoneNumber] nvarchar(10) NOT NULL,
    [RecipientAddressLine] nvarchar(256) NOT NULL,
    [RecipientProvince] nvarchar(max) NOT NULL,
    [RecipientWard] nvarchar(max) NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [SellerId] uniqueidentifier NOT NULL,
    [ExpiredAt] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_UserProfiles_CustomerId] FOREIGN KEY ([CustomerId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_UserProfiles_SellerId] FOREIGN KEY ([SellerId])
        REFERENCES [business].[UserProfiles] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Orders_Code]
    ON [business].[Orders] ([Code]);
GO

CREATE INDEX [IX_Orders_CreatedAt]
    ON [business].[Orders] ([CreatedAt]);
GO

CREATE INDEX [IX_Orders_CustomerId]
    ON [business].[Orders] ([CustomerId]);
GO

CREATE INDEX [IX_Orders_SellerId]
    ON [business].[Orders] ([SellerId]);
GO

CREATE INDEX [IX_Orders_Status]
    ON [business].[Orders] ([Status]);
GO

CREATE TABLE [business].[OrderItems]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [CurrentAmount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId])
        REFERENCES [business].[Orders] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [business].[Products] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_OrderItems_OrderId_ProductId]
    ON [business].[OrderItems] ([OrderId], [ProductId]);
GO

CREATE INDEX [IX_OrderItems_ProductId]
    ON [business].[OrderItems] ([ProductId]);
GO

CREATE TABLE [business].[Payments]
(
    [Id] uniqueidentifier NOT NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Method] int NOT NULL,
    [Provider] nvarchar(128) NULL,
    [Status] int NOT NULL,
    [ExternalTransactionId] nvarchar(256) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY ([OrderId])
        REFERENCES [business].[Orders] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Payments_OrderId]
    ON [business].[Payments] ([OrderId]);
GO

CREATE INDEX [IX_Payments_Status]
    ON [business].[Payments] ([Status]);
GO

CREATE INDEX [IX_Payments_ExternalTransactionId]
    ON [business].[Payments] ([ExternalTransactionId]);
GO

CREATE TABLE [translation].[Translations]
(
    [Id] uniqueidentifier NOT NULL,
    [EntityName] nvarchar(128) NOT NULL,
    [FieldName] nvarchar(128) NOT NULL,
    [RowId] uniqueidentifier NOT NULL,
    [Locale] nvarchar(16) NOT NULL,
    [Value] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Translations] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Translations_EntityName_FieldName_RowId_Locale]
    ON [translation].[Translations] ([EntityName], [FieldName], [RowId], [Locale]);
GO