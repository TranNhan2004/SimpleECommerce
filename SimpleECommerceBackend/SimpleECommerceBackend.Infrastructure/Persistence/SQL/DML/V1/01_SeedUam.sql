SET NOCOUNT ON;
GO

DECLARE @SeededAt datetimeoffset = '2026-05-26T06:21:10+00:00';
DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000001';
DECLARE @AdminUserId uniqueidentifier = '019e62f1-bbfe-74fe-91f2-c920ddb69ff8';

-- Users
INSERT INTO [uam].[Users]
(
    [Id],
    [KeycloakSubjectId],
    [Email],
    [FirstName],
    [LastName],
    [NickName],
    [Sex],
    [Status],
    [BirthDate],
    [AvatarUrl],
    [CreatedAt],
    [CreatedById],
    [UpdatedAt],
    [UpdatedById],
    [IsDeleted],
    [DeletedAt],
    [DeletedById]
)
VALUES
(
    @SystemUserId,
    @SystemUserId,
    N'system@simpleecommerce.local',
    N'System',
    N'User',
    N'system',
    3,
    1,
    '1990-01-01',
    NULL,
    @SeededAt,
    @SystemUserId,
    NULL,
    NULL,
    0,
    NULL,
    NULL
);

INSERT INTO [uam].[Users]
(
    [Id],
    [KeycloakSubjectId],
    [Email],
    [FirstName],
    [LastName],
    [NickName],
    [Sex],
    [Status],
    [BirthDate],
    [AvatarUrl],
    [CreatedAt],
    [CreatedById],
    [UpdatedAt],
    [UpdatedById],
    [IsDeleted],
    [DeletedAt],
    [DeletedById]
)
VALUES
(
    @AdminUserId,
    '313ce163-e7f6-4243-99de-e7816b5ccabd',
    N'admin@test.com',
    N'Admin',
    N'User',
    N'admin@test.com',
    3,
    1,
    '1990-01-01',
    NULL,
    @SeededAt,
    @SystemUserId,
    NULL,
    NULL,
    0,
    NULL,
    NULL
);

-- Roles
INSERT INTO [uam].[Roles] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf0-755f-9583-21f5998c49b1', N'admin', N'Admin', N'Full administrative access.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Roles] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf1-7a89-9e5d-58023679e0b0', N'customer', N'Customer', N'Customer access for self-service features.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Roles] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf2-7e43-b26a-4bdeed3ff2c4', N'seller', N'Seller', N'Seller access for storefront-related features.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

-- Permissions
INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf3-7675-a305-441040a1930d', N'users.self.create', N'Users Self Create', N'Allows a user to create their own profile.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf4-73ef-abbf-45175f9a9783', N'users.self.read', N'Users Self Read', N'Allows a user to view their own profile.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf5-757d-a980-433357496b16', N'users.self.update', N'Users Self Update', N'Allows a user to update their own profile.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf6-705e-8221-9b2c66297cc9', N'users.self.activate', N'Users Self Activate', N'Allows a user to activate their own profile.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf7-7418-8427-71a0bc15da75', N'users.self.delete', N'Users Self Delete', N'Allows a user to delete their own profile.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf8-788c-8a82-cf8dd084494b', N'permissions.self.read', N'Permissions Self Read', N'Allows a user to view their granted permissions.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbf9-7c30-8f71-a2aeba47206e', N'categories.read', N'Categories Read', N'Allows a user to view public categories.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbfa-7872-8b29-6265c135234a', N'categories.read.admin', N'Categories Read Admin', N'Allows a user to view admin category data.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbfb-7e23-8efe-8f7a82c533f9', N'categories.create', N'Categories Create', N'Allows a user to create categories.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbfc-7c86-81e7-312e27fb4367', N'categories.update', N'Categories Update', N'Allows a user to update categories.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbfd-750d-943f-6899db79d0cf', N'categories.delete', N'Categories Delete', N'Allows a user to delete categories.', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

-- User roles
INSERT INTO [uam].[UserRoles] ([Id], [UserId], [RoleId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bbff-7f60-a5d7-23ca4e6d24b3', @AdminUserId, '019e62f1-bbf0-755f-9583-21f5998c49b1', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

-- Admin role permissions
INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc00-71b3-8112-29737d997942', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbf3-7675-a305-441040a1930d', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc01-733d-a646-f6eaf64b5421', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbf4-73ef-abbf-45175f9a9783', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc02-7d84-a5b4-c278ab669583', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbf5-757d-a980-433357496b16', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc03-76c6-8a06-42d10dc1d944', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbf6-705e-8221-9b2c66297cc9', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc04-7572-8bf5-ee717f32acf3', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbf7-7418-8427-71a0bc15da75', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc05-758e-9bd5-24d3a07c68e7', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbf8-788c-8a82-cf8dd084494b', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc06-7dea-982c-9df06ffd2503', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbf9-7c30-8f71-a2aeba47206e', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc07-73d3-b62b-9e4121dcb00b', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbfa-7872-8b29-6265c135234a', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc08-792c-a72e-73c633c4b01f', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbfb-7e23-8efe-8f7a82c533f9', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc09-7934-8a9d-6be53474aee7', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbfc-7c86-81e7-312e27fb4367', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc0a-7a94-b0ab-6a8d3223e71b', '019e62f1-bbf0-755f-9583-21f5998c49b1', '019e62f1-bbfd-750d-943f-6899db79d0cf', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

-- Customer role permissions
INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc0b-710e-8ae0-c0d97fc91721', '019e62f1-bbf1-7a89-9e5d-58023679e0b0', '019e62f1-bbf4-73ef-abbf-45175f9a9783', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc0c-74ea-8dac-1c9c310661cc', '019e62f1-bbf1-7a89-9e5d-58023679e0b0', '019e62f1-bbf5-757d-a980-433357496b16', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc15-741f-937a-75d842612c17', '019e62f1-bbf1-7a89-9e5d-58023679e0b0', '019e62f1-bbf6-705e-8221-9b2c66297cc9', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc0d-7bca-8d27-5d4a2f23d50a', '019e62f1-bbf1-7a89-9e5d-58023679e0b0', '019e62f1-bbf7-7418-8427-71a0bc15da75', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc0e-7405-8273-07faffab497e', '019e62f1-bbf1-7a89-9e5d-58023679e0b0', '019e62f1-bbf8-788c-8a82-cf8dd084494b', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc0f-7a8b-821d-07d2f3ded8a9', '019e62f1-bbf1-7a89-9e5d-58023679e0b0', '019e62f1-bbf9-7c30-8f71-a2aeba47206e', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

-- Seller role permissions
INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc10-7ecb-b694-c7d40ef9fa35', '019e62f1-bbf2-7e43-b26a-4bdeed3ff2c4', '019e62f1-bbf4-73ef-abbf-45175f9a9783', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc11-78d6-aca3-3b53ae077d96', '019e62f1-bbf2-7e43-b26a-4bdeed3ff2c4', '019e62f1-bbf5-757d-a980-433357496b16', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc16-7d52-9c4c-b21846086f4e', '019e62f1-bbf2-7e43-b26a-4bdeed3ff2c4', '019e62f1-bbf6-705e-8221-9b2c66297cc9', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc12-7c5a-b2b6-8550b6fccaee', '019e62f1-bbf2-7e43-b26a-4bdeed3ff2c4', '019e62f1-bbf7-7418-8427-71a0bc15da75', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc13-7096-bf0b-05db79f32c9a', '019e62f1-bbf2-7e43-b26a-4bdeed3ff2c4', '019e62f1-bbf8-788c-8a82-cf8dd084494b', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);

INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedById], [UpdatedAt], [UpdatedById], [IsDeleted], [DeletedAt], [DeletedById])
VALUES ('019e62f1-bc14-79eb-a105-eac0a001bcb9', '019e62f1-bbf2-7e43-b26a-4bdeed3ff2c4', '019e62f1-bbf9-7c30-8f71-a2aeba47206e', @SeededAt, @SystemUserId, NULL, NULL, 0, NULL, NULL);
GO
