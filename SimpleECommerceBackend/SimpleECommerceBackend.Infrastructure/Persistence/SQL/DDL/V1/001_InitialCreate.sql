IF SCHEMA_ID(N'business') IS NULL
    EXEC(N'CREATE SCHEMA business');
GO

IF SCHEMA_ID(N'uam') IS NULL
    EXEC(N'CREATE SCHEMA uam');
GO

IF SCHEMA_ID(N'translation') IS NULL
    EXEC(N'CREATE SCHEMA translation');
GO

CREATE TABLE [uam].[Users]
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
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Users_Email]
    ON [uam].[Users] ([Email])
    WHERE [Status] <> 99;
GO

CREATE TABLE [uam].[Roles]
(
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(64) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Description] nvarchar(512) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Roles_Code]
    ON [uam].[Roles] ([Code]);
GO

CREATE TABLE [uam].[Permissions]
(
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(128) NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(512) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Permissions_Code]
    ON [uam].[Permissions] ([Code]);
GO

CREATE TABLE [uam].[UserRoles]
(
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [uam].[Roles] ([Id])
        ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_UserRoles_UserId_RoleId]
    ON [uam].[UserRoles] ([UserId], [RoleId]);
GO

CREATE TABLE [uam].[RolePermissions]
(
    [Id] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    [PermissionId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [uam].[Roles] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId])
        REFERENCES [uam].[Permissions] ([Id])
        ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_RolePermissions_RoleId_PermissionId]
    ON [uam].[RolePermissions] ([RoleId], [PermissionId]);
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
    CONSTRAINT [FK_Categories_Users_AdminId] FOREIGN KEY ([AdminId])
        REFERENCES [uam].[Users] ([Id])
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
    CONSTRAINT [FK_SellerShops_Users_SellerId] FOREIGN KEY ([SellerId])
        REFERENCES [uam].[Users] ([Id])
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
    [AverageRating] decimal(3,2) NOT NULL,
    [TotalRatings] int NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [SellerId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId])
        REFERENCES [business].[Categories] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Products_Users_SellerId] FOREIGN KEY ([SellerId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Products_CategoryId]
    ON [business].[Products] ([CategoryId]);
GO

CREATE INDEX [IX_Products_SellerId]
    ON [business].[Products] ([SellerId]);
GO

CREATE TABLE [business].[ProductVariants]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [Name] nvarchar(512) NOT NULL,
    [Description] nvarchar(2048) NOT NULL,
    [CurrentAmount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [TotalInStock] int NOT NULL,
    [DefaultImageUrl] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_ProductVariants] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariants_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [business].[Products] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_ProductVariants_ProductId]
    ON [business].[ProductVariants] ([ProductId]);
GO

CREATE TABLE [business].[ProductVariantImages]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductVariantId] uniqueidentifier NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsDisplayed] bit NOT NULL CONSTRAINT [DF_ProductVariantImages_IsDisplayed] DEFAULT (1),
    [Description] nvarchar(512) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_ProductVariantImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariantImages_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId])
        REFERENCES [business].[ProductVariants] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ProductVariantImages_ProductVariantId]
    ON [business].[ProductVariantImages] ([ProductVariantId]);
GO

CREATE TABLE [business].[ProductVariantPrices]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductVariantId] uniqueidentifier NOT NULL,
    [EffectiveFrom] datetimeoffset NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_ProductVariantPrices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariantPrices_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId])
        REFERENCES [business].[ProductVariants] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ProductVariantPrices_ProductVariantId]
    ON [business].[ProductVariantPrices] ([ProductVariantId]);
GO

CREATE TABLE [business].[Reviews]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [Rating] int NOT NULL,
    [Comment] nvarchar(2048) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [business].[Products] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Users_CustomerId] FOREIGN KEY ([CustomerId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Reviews_ProductId]
    ON [business].[Reviews] ([ProductId]);
GO

CREATE INDEX [IX_Reviews_CustomerId]
    ON [business].[Reviews] ([CustomerId]);
GO

CREATE TABLE [business].[ReviewResponses]
(
    [Id] uniqueidentifier NOT NULL,
    [ReviewId] uniqueidentifier NOT NULL,
    [FromUserId] uniqueidentifier NOT NULL,
    [ToUserId] uniqueidentifier NOT NULL,
    [Comment] nvarchar(2048) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_ReviewResponses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ReviewResponses_Reviews_ReviewId] FOREIGN KEY ([ReviewId])
        REFERENCES [business].[Reviews] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_ReviewResponses_Users_FromUserId] FOREIGN KEY ([FromUserId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_ReviewResponses_Users_ToUserId] FOREIGN KEY ([ToUserId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_ReviewResponses_ReviewId]
    ON [business].[ReviewResponses] ([ReviewId]);
GO

CREATE INDEX [IX_ReviewResponses_FromUserId]
    ON [business].[ReviewResponses] ([FromUserId]);
GO

CREATE INDEX [IX_ReviewResponses_ToUserId]
    ON [business].[ReviewResponses] ([ToUserId]);
GO

CREATE TABLE [business].[Carts]
(
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Carts_Users_CustomerId] FOREIGN KEY ([CustomerId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Carts_CustomerId]
    ON [business].[Carts] ([CustomerId]);
GO

CREATE TABLE [business].[CartItems]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductVariantId] uniqueidentifier NOT NULL,
    [CartId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId])
        REFERENCES [business].[ProductVariants] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId])
        REFERENCES [business].[Carts] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CartItems_CartId]
    ON [business].[CartItems] ([CartId]);
GO

CREATE INDEX [IX_CartItems_ProductVariantId]
    ON [business].[CartItems] ([ProductVariantId]);
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
    CONSTRAINT [FK_CustomerShippingAddresses_Users_CustomerId] FOREIGN KEY ([CustomerId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CustomerShippingAddresses_CustomerId]
    ON [business].[CustomerShippingAddresses] ([CustomerId]);
GO

CREATE TABLE [business].[Inventories]
(
    [Id] uniqueidentifier NOT NULL,
    [ProductVariantId] uniqueidentifier NOT NULL,
    [SellerWarehouseId] uniqueidentifier NOT NULL,
    [QuantityInStock] int NOT NULL,
    [QuantityReserved] int NOT NULL,
    [Version] int NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    CONSTRAINT [PK_Inventories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Inventories_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId])
        REFERENCES [business].[ProductVariants] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_Inventories_SellerWarehouses_SellerWarehouseId] FOREIGN KEY ([SellerWarehouseId])
        REFERENCES [business].[SellerWarehouses] ([Id])
        ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Inventories_ProductVariantId_SellerWarehouseId]
    ON [business].[Inventories] ([ProductVariantId], [SellerWarehouseId]);
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
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId])
        REFERENCES [uam].[Users] ([Id])
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
    CONSTRAINT [FK_Orders_Users_CustomerId] FOREIGN KEY ([CustomerId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_Users_SellerId] FOREIGN KEY ([SellerId])
        REFERENCES [uam].[Users] ([Id])
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
    [ProductVariantId] uniqueidentifier NOT NULL,
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
    CONSTRAINT [FK_OrderItems_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId])
        REFERENCES [business].[ProductVariants] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_OrderItems_OrderId_ProductVariantId]
    ON [business].[OrderItems] ([OrderId], [ProductVariantId]);
GO

CREATE INDEX [IX_OrderItems_ProductVariantId]
    ON [business].[OrderItems] ([ProductVariantId]);
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
