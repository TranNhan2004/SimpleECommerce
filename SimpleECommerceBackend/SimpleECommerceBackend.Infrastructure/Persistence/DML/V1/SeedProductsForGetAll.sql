SET NOCOUNT ON;
GO

/*
    Seed data for testing the customer get-all-products API.
    This script is idempotent and assumes:
    1. DDL/V1/InitialCreate.sql has been applied.
    2. DDL/V1/UpdateProduct.sql has been applied.

    It inserts prerequisite rows for:
    - business.UserProfiles
    - business.SellerShops
    - business.SellerWarehouses

    Then it inserts target rows for:
    - business.Categories
    - business.Products
    - business.ProductPrices
    - business.ProductImages
    - business.Inventories
*/

IF NOT EXISTS (
    SELECT 1
    FROM [business].[UserProfiles]
    WHERE [Id] = 'e5d28ee9-3c85-4462-a14e-79714ace19c2'
)
BEGIN
    INSERT INTO [business].[UserProfiles]
    (
        [Id],
        [Email],
        [FirstName],
        [LastName],
        [NickName],
        [Sex],
        [Status],
        [BirthDate],
        [AvatarUrl],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        'e5d28ee9-3c85-4462-a14e-79714ace19c2',
        'seed.admin@getall.local',
        'Catalog',
        'Admin',
        'catalog-admin',
        1,
        1,
        '1990-01-15',
        'https://cdn.example.com/avatars/catalog-admin.png',
        '2026-05-01T08:00:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[UserProfiles]
    WHERE [Id] = 'e760ecdd-0c2d-4c95-962c-c81d1a9eea38'
)
BEGIN
    INSERT INTO [business].[UserProfiles]
    (
        [Id],
        [Email],
        [FirstName],
        [LastName],
        [NickName],
        [Sex],
        [Status],
        [BirthDate],
        [AvatarUrl],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        'e760ecdd-0c2d-4c95-962c-c81d1a9eea38',
        'seed.seller1@getall.local',
        'Linh',
        'Nguyen',
        'linh-store',
        2,
        1,
        '1994-03-12',
        'https://cdn.example.com/avatars/linh-nguyen.png',
        '2026-05-01T08:05:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[UserProfiles]
    WHERE [Id] = 'e90a0c1d-3248-4d61-9d58-bb7eb46d500d'
)
BEGIN
    INSERT INTO [business].[UserProfiles]
    (
        [Id],
        [Email],
        [FirstName],
        [LastName],
        [NickName],
        [Sex],
        [Status],
        [BirthDate],
        [AvatarUrl],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        'e90a0c1d-3248-4d61-9d58-bb7eb46d500d',
        'seed.seller2@getall.local',
        'Minh',
        'Tran',
        'minh-home',
        1,
        1,
        '1992-09-08',
        'https://cdn.example.com/avatars/minh-tran.png',
        '2026-05-01T08:10:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[SellerShops]
    WHERE [Id] = '999c18eb-fcfd-44b4-9ca3-d0de0e676cd7'
)
BEGIN
    INSERT INTO [business].[SellerShops]
    (
        [Id],
        [SellerId],
        [Name],
        [PhoneNumber],
        [AvatarUrl],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '999c18eb-fcfd-44b4-9ca3-d0de0e676cd7',
        'e760ecdd-0c2d-4c95-962c-c81d1a9eea38',
        'Linh Tech Store',
        '0901234567',
        'https://cdn.example.com/shops/linh-tech-store.png',
        '2026-05-01T08:20:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[SellerShops]
    WHERE [Id] = '5aab231c-b19d-445a-8be2-e355cb7a23a6'
)
BEGIN
    INSERT INTO [business].[SellerShops]
    (
        [Id],
        [SellerId],
        [Name],
        [PhoneNumber],
        [AvatarUrl],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '5aab231c-b19d-445a-8be2-e355cb7a23a6',
        'e90a0c1d-3248-4d61-9d58-bb7eb46d500d',
        'Minh Home Living',
        '0907654321',
        'https://cdn.example.com/shops/minh-home-living.png',
        '2026-05-01T08:25:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[SellerWarehouses]
    WHERE [Id] = '3a909be4-5729-4796-af10-285cd13d8524'
)
BEGIN
    INSERT INTO [business].[SellerWarehouses]
    (
        [Id],
        [SellerShopId],
        [AddressLine],
        [Province],
        [Ward],
        [CreatedAt],
        [IsDeleted],
        [DeletedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '3a909be4-5729-4796-af10-285cd13d8524',
        '999c18eb-fcfd-44b4-9ca3-d0de0e676cd7',
        '12 Nguyen Hue Street',
        'Ho Chi Minh City',
        'Ben Nghe',
        '2026-05-01T08:30:00+00:00',
        0,
        NULL,
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[SellerWarehouses]
    WHERE [Id] = '7e3d6a83-e1b1-4ea6-b541-91a5b00194b7'
)
BEGIN
    INSERT INTO [business].[SellerWarehouses]
    (
        [Id],
        [SellerShopId],
        [AddressLine],
        [Province],
        [Ward],
        [CreatedAt],
        [IsDeleted],
        [DeletedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '7e3d6a83-e1b1-4ea6-b541-91a5b00194b7',
        '999c18eb-fcfd-44b4-9ca3-d0de0e676cd7',
        '88 Cach Mang Thang 8',
        'Ho Chi Minh City',
        'Ward 6',
        '2026-05-01T08:35:00+00:00',
        0,
        NULL,
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[SellerWarehouses]
    WHERE [Id] = '5c82dca0-d909-4402-9205-5519f834f656'
)
BEGIN
    INSERT INTO [business].[SellerWarehouses]
    (
        [Id],
        [SellerShopId],
        [AddressLine],
        [Province],
        [Ward],
        [CreatedAt],
        [IsDeleted],
        [DeletedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '5c82dca0-d909-4402-9205-5519f834f656',
        '5aab231c-b19d-445a-8be2-e355cb7a23a6',
        '25 Tran Hung Dao',
        'Da Nang',
        'Hai Chau 1',
        '2026-05-01T08:40:00+00:00',
        0,
        NULL,
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Categories]
    WHERE [Id] = '2c10d798-d803-4638-a79b-dee1f46ce526'
)
BEGIN
    INSERT INTO [business].[Categories]
    (
        [Id],
        [Name],
        [Description],
        [Status],
        [AdminId],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '2c10d798-d803-4638-a79b-dee1f46ce526',
        'Computer Accessories',
        'Seed category for accessories and office gear.',
        1,
        'e5d28ee9-3c85-4462-a14e-79714ace19c2',
        '2026-05-01T09:00:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Categories]
    WHERE [Id] = 'f9d2305a-2d5e-4e52-acb1-ec0924d7cdec'
)
BEGIN
    INSERT INTO [business].[Categories]
    (
        [Id],
        [Name],
        [Description],
        [Status],
        [AdminId],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        'f9d2305a-2d5e-4e52-acb1-ec0924d7cdec',
        'Home Living',
        'Seed category for home and decor items.',
        1,
        'e5d28ee9-3c85-4462-a14e-79714ace19c2',
        '2026-05-01T09:05:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Products]
    WHERE [Id] = '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889'
)
BEGIN
    INSERT INTO [business].[Products]
    (
        [Id],
        [Name],
        [Description],
        [CurrentAmount],
        [Currency],
        [TotalInStock],
        [Status],
        [CategoryId],
        [SellerId],
        [CreatedAt],
        [UpdatedAt],
        [AverageRating],
        [TotalRatings],
        [DefaultImageUrl]
    )
    VALUES
    (
        '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889',
        'Ergonomic Wireless Mouse',
        'Silent wireless mouse with USB receiver and two thumb buttons.',
        29.99,
        'USD',
        35,
        2,
        '2c10d798-d803-4638-a79b-dee1f46ce526',
        'e760ecdd-0c2d-4c95-962c-c81d1a9eea38',
        '2026-05-02T09:00:00+00:00',
        '2026-05-05T10:30:00+00:00',
        4.70,
        18,
        'https://cdn.example.com/products/mouse-1/main.png'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Products]
    WHERE [Id] = '25a49033-2f5a-49d4-851e-c9da31056615'
)
BEGIN
    INSERT INTO [business].[Products]
    (
        [Id],
        [Name],
        [Description],
        [CurrentAmount],
        [Currency],
        [TotalInStock],
        [Status],
        [CategoryId],
        [SellerId],
        [CreatedAt],
        [UpdatedAt],
        [AverageRating],
        [TotalRatings],
        [DefaultImageUrl]
    )
    VALUES
    (
        '25a49033-2f5a-49d4-851e-c9da31056615',
        'Compact Mechanical Keyboard',
        'Seventy-five percent mechanical keyboard with hot-swappable switches.',
        89.50,
        'USD',
        12,
        2,
        '2c10d798-d803-4638-a79b-dee1f46ce526',
        'e760ecdd-0c2d-4c95-962c-c81d1a9eea38',
        '2026-05-02T09:15:00+00:00',
        '2026-05-05T11:00:00+00:00',
        4.85,
        31,
        'https://cdn.example.com/products/keyboard-1/main.png'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Products]
    WHERE [Id] = '87e69cc5-397f-4108-93e3-29f97d362ae3'
)
BEGIN
    INSERT INTO [business].[Products]
    (
        [Id],
        [Name],
        [Description],
        [CurrentAmount],
        [Currency],
        [TotalInStock],
        [Status],
        [CategoryId],
        [SellerId],
        [CreatedAt],
        [UpdatedAt],
        [AverageRating],
        [TotalRatings],
        [DefaultImageUrl]
    )
    VALUES
    (
        '87e69cc5-397f-4108-93e3-29f97d362ae3',
        '7-in-1 USB-C Hub',
        'Portable hub with HDMI, USB-A, USB-C PD, and SD card slots.',
        45.00,
        'USD',
        27,
        2,
        '2c10d798-d803-4638-a79b-dee1f46ce526',
        'e90a0c1d-3248-4d61-9d58-bb7eb46d500d',
        '2026-05-02T09:30:00+00:00',
        '2026-05-05T11:20:00+00:00',
        4.55,
        14,
        'https://cdn.example.com/products/hub-1/main.png'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Products]
    WHERE [Id] = '785ed593-4552-4ef4-9cf7-93032a8a4d58'
)
BEGIN
    INSERT INTO [business].[Products]
    (
        [Id],
        [Name],
        [Description],
        [CurrentAmount],
        [Currency],
        [TotalInStock],
        [Status],
        [CategoryId],
        [SellerId],
        [CreatedAt],
        [UpdatedAt],
        [AverageRating],
        [TotalRatings],
        [DefaultImageUrl]
    )
    VALUES
    (
        '785ed593-4552-4ef4-9cf7-93032a8a4d58',
        'Ceramic Desk Planter',
        'Minimal ceramic planter for desks and shelves.',
        24.75,
        'USD',
        8,
        3,
        'f9d2305a-2d5e-4e52-acb1-ec0924d7cdec',
        'e90a0c1d-3248-4d61-9d58-bb7eb46d500d',
        '2026-05-02T09:45:00+00:00',
        NULL,
        4.10,
        6,
        'https://cdn.example.com/products/planter-1/main.png'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductPrices]
    WHERE [Id] = 'a0043d8b-2e06-406a-aab9-9f1f99c6d28c'
)
BEGIN
    INSERT INTO [business].[ProductPrices]
    (
        [Id],
        [ProductId],
        [EffectiveFrom],
        [Amount],
        [Currency],
        [CreatedAt]
    )
    VALUES
    (
        'a0043d8b-2e06-406a-aab9-9f1f99c6d28c',
        '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889',
        '2026-04-20T00:00:00+00:00',
        34.99,
        'USD',
        '2026-04-20T00:00:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductPrices]
    WHERE [Id] = 'e69519e1-b8ef-4b66-8e6d-bcda53819e85'
)
BEGIN
    INSERT INTO [business].[ProductPrices]
    (
        [Id],
        [ProductId],
        [EffectiveFrom],
        [Amount],
        [Currency],
        [CreatedAt]
    )
    VALUES
    (
        'e69519e1-b8ef-4b66-8e6d-bcda53819e85',
        '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889',
        '2026-05-01T00:00:00+00:00',
        29.99,
        'USD',
        '2026-05-01T00:00:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductPrices]
    WHERE [Id] = '1ee1b912-3859-49ed-96be-65448770042f'
)
BEGIN
    INSERT INTO [business].[ProductPrices]
    (
        [Id],
        [ProductId],
        [EffectiveFrom],
        [Amount],
        [Currency],
        [CreatedAt]
    )
    VALUES
    (
        '1ee1b912-3859-49ed-96be-65448770042f',
        '25a49033-2f5a-49d4-851e-c9da31056615',
        '2026-04-18T00:00:00+00:00',
        95.00,
        'USD',
        '2026-04-18T00:00:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductPrices]
    WHERE [Id] = '001fa6af-e75c-41d8-8c8a-a44987cf8596'
)
BEGIN
    INSERT INTO [business].[ProductPrices]
    (
        [Id],
        [ProductId],
        [EffectiveFrom],
        [Amount],
        [Currency],
        [CreatedAt]
    )
    VALUES
    (
        '001fa6af-e75c-41d8-8c8a-a44987cf8596',
        '25a49033-2f5a-49d4-851e-c9da31056615',
        '2026-05-01T00:00:00+00:00',
        89.50,
        'USD',
        '2026-05-01T00:00:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductPrices]
    WHERE [Id] = 'f29cd2f1-dd99-40da-b0b5-53cf42f32dc9'
)
BEGIN
    INSERT INTO [business].[ProductPrices]
    (
        [Id],
        [ProductId],
        [EffectiveFrom],
        [Amount],
        [Currency],
        [CreatedAt]
    )
    VALUES
    (
        'f29cd2f1-dd99-40da-b0b5-53cf42f32dc9',
        '87e69cc5-397f-4108-93e3-29f97d362ae3',
        '2026-04-25T00:00:00+00:00',
        49.00,
        'USD',
        '2026-04-25T00:00:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductPrices]
    WHERE [Id] = '807b0022-c379-4866-b213-0fbf8631fde4'
)
BEGIN
    INSERT INTO [business].[ProductPrices]
    (
        [Id],
        [ProductId],
        [EffectiveFrom],
        [Amount],
        [Currency],
        [CreatedAt]
    )
    VALUES
    (
        '807b0022-c379-4866-b213-0fbf8631fde4',
        '87e69cc5-397f-4108-93e3-29f97d362ae3',
        '2026-05-01T00:00:00+00:00',
        45.00,
        'USD',
        '2026-05-01T00:00:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductPrices]
    WHERE [Id] = '8cf07925-fce2-410c-b1d5-649063305f31'
)
BEGIN
    INSERT INTO [business].[ProductPrices]
    (
        [Id],
        [ProductId],
        [EffectiveFrom],
        [Amount],
        [Currency],
        [CreatedAt]
    )
    VALUES
    (
        '8cf07925-fce2-410c-b1d5-649063305f31',
        '785ed593-4552-4ef4-9cf7-93032a8a4d58',
        '2026-05-01T00:00:00+00:00',
        24.75,
        'USD',
        '2026-05-01T00:00:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductImages]
    WHERE [Id] = 'f03a311d-2852-4257-97a9-522a74f8af6f'
)
BEGIN
    INSERT INTO [business].[ProductImages]
    (
        [Id],
        [ProductId],
        [ImageUrl],
        [DisplayOrder],
        [IsDisplayed],
        [Description],
        [CreatedAt]
    )
    VALUES
    (
        'f03a311d-2852-4257-97a9-522a74f8af6f',
        '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889',
        'https://cdn.example.com/products/mouse-1/main.png',
        1,
        1,
        'Main product image',
        '2026-05-02T09:05:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductImages]
    WHERE [Id] = '30630611-27ff-43c0-8998-cf9d7d441ca8'
)
BEGIN
    INSERT INTO [business].[ProductImages]
    (
        [Id],
        [ProductId],
        [ImageUrl],
        [DisplayOrder],
        [IsDisplayed],
        [Description],
        [CreatedAt]
    )
    VALUES
    (
        '30630611-27ff-43c0-8998-cf9d7d441ca8',
        '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889',
        'https://cdn.example.com/products/mouse-1/side.png',
        2,
        1,
        'Side angle image',
        '2026-05-02T09:06:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductImages]
    WHERE [Id] = 'fa3233fe-9ad7-4b18-87f9-56fec6d75c95'
)
BEGIN
    INSERT INTO [business].[ProductImages]
    (
        [Id],
        [ProductId],
        [ImageUrl],
        [DisplayOrder],
        [IsDisplayed],
        [Description],
        [CreatedAt]
    )
    VALUES
    (
        'fa3233fe-9ad7-4b18-87f9-56fec6d75c95',
        '25a49033-2f5a-49d4-851e-c9da31056615',
        'https://cdn.example.com/products/keyboard-1/main.png',
        1,
        1,
        'Main product image',
        '2026-05-02T09:20:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductImages]
    WHERE [Id] = 'aff6f040-bcb2-4aae-8a8a-0f37c153bd4e'
)
BEGIN
    INSERT INTO [business].[ProductImages]
    (
        [Id],
        [ProductId],
        [ImageUrl],
        [DisplayOrder],
        [IsDisplayed],
        [Description],
        [CreatedAt]
    )
    VALUES
    (
        'aff6f040-bcb2-4aae-8a8a-0f37c153bd4e',
        '25a49033-2f5a-49d4-851e-c9da31056615',
        'https://cdn.example.com/products/keyboard-1/angled.png',
        2,
        1,
        'Angled keyboard image',
        '2026-05-02T09:21:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductImages]
    WHERE [Id] = 'fa307fc1-64e0-4279-86d6-5134dbbd0612'
)
BEGIN
    INSERT INTO [business].[ProductImages]
    (
        [Id],
        [ProductId],
        [ImageUrl],
        [DisplayOrder],
        [IsDisplayed],
        [Description],
        [CreatedAt]
    )
    VALUES
    (
        'fa307fc1-64e0-4279-86d6-5134dbbd0612',
        '87e69cc5-397f-4108-93e3-29f97d362ae3',
        'https://cdn.example.com/products/hub-1/main.png',
        1,
        1,
        'Main product image',
        '2026-05-02T09:35:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[ProductImages]
    WHERE [Id] = 'bf6b8560-4c7a-4653-85c2-a0cd1a0d1d4c'
)
BEGIN
    INSERT INTO [business].[ProductImages]
    (
        [Id],
        [ProductId],
        [ImageUrl],
        [DisplayOrder],
        [IsDisplayed],
        [Description],
        [CreatedAt]
    )
    VALUES
    (
        'bf6b8560-4c7a-4653-85c2-a0cd1a0d1d4c',
        '785ed593-4552-4ef4-9cf7-93032a8a4d58',
        'https://cdn.example.com/products/planter-1/main.png',
        1,
        1,
        'Main product image',
        '2026-05-02T09:50:00+00:00'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Inventories]
    WHERE [Id] = '4086386f-5128-4f85-931c-e4995a083c25'
)
BEGIN
    INSERT INTO [business].[Inventories]
    (
        [Id],
        [ProductId],
        [SellerWarehouseId],
        [QuantityInStock],
        [QuantityReserved],
        [Version],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '4086386f-5128-4f85-931c-e4995a083c25',
        '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889',
        '3a909be4-5729-4796-af10-285cd13d8524',
        20,
        3,
        1,
        '2026-05-02T10:00:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Inventories]
    WHERE [Id] = '06ba9195-94a7-406b-8cd0-df3a0945e315'
)
BEGIN
    INSERT INTO [business].[Inventories]
    (
        [Id],
        [ProductId],
        [SellerWarehouseId],
        [QuantityInStock],
        [QuantityReserved],
        [Version],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '06ba9195-94a7-406b-8cd0-df3a0945e315',
        '2ca94eaf-95d1-4fb0-bc92-cab49bc2e889',
        '7e3d6a83-e1b1-4ea6-b541-91a5b00194b7',
        15,
        1,
        1,
        '2026-05-02T10:01:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Inventories]
    WHERE [Id] = '17ea7138-1b6e-4db3-87ff-3413579bbd34'
)
BEGIN
    INSERT INTO [business].[Inventories]
    (
        [Id],
        [ProductId],
        [SellerWarehouseId],
        [QuantityInStock],
        [QuantityReserved],
        [Version],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '17ea7138-1b6e-4db3-87ff-3413579bbd34',
        '25a49033-2f5a-49d4-851e-c9da31056615',
        '3a909be4-5729-4796-af10-285cd13d8524',
        5,
        0,
        1,
        '2026-05-02T10:02:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Inventories]
    WHERE [Id] = 'a711b77f-f338-4868-9838-94d64687677f'
)
BEGIN
    INSERT INTO [business].[Inventories]
    (
        [Id],
        [ProductId],
        [SellerWarehouseId],
        [QuantityInStock],
        [QuantityReserved],
        [Version],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        'a711b77f-f338-4868-9838-94d64687677f',
        '25a49033-2f5a-49d4-851e-c9da31056615',
        '7e3d6a83-e1b1-4ea6-b541-91a5b00194b7',
        7,
        2,
        1,
        '2026-05-02T10:03:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Inventories]
    WHERE [Id] = '832c3a13-23dd-482d-8f46-06dca7cde971'
)
BEGIN
    INSERT INTO [business].[Inventories]
    (
        [Id],
        [ProductId],
        [SellerWarehouseId],
        [QuantityInStock],
        [QuantityReserved],
        [Version],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '832c3a13-23dd-482d-8f46-06dca7cde971',
        '87e69cc5-397f-4108-93e3-29f97d362ae3',
        '5c82dca0-d909-4402-9205-5519f834f656',
        27,
        4,
        1,
        '2026-05-02T10:04:00+00:00',
        NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM [business].[Inventories]
    WHERE [Id] = '38413ec8-9edf-4800-8e9e-9073bb48b9f7'
)
BEGIN
    INSERT INTO [business].[Inventories]
    (
        [Id],
        [ProductId],
        [SellerWarehouseId],
        [QuantityInStock],
        [QuantityReserved],
        [Version],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES
    (
        '38413ec8-9edf-4800-8e9e-9073bb48b9f7',
        '785ed593-4552-4ef4-9cf7-93032a8a4d58',
        '5c82dca0-d909-4402-9205-5519f834f656',
        8,
        0,
        1,
        '2026-05-02T10:05:00+00:00',
        NULL
    );
END;
GO
