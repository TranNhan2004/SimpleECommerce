/*
    Seed baseline UAM roles and permissions.

    Assumptions:
    1. DDL/V1/001_InitialCreate.sql has been applied.
*/

SET NOCOUNT ON;

DECLARE @AdminRoleId uniqueidentifier = '70000000-0000-0000-0000-000000000001';
DECLARE @SellerRoleId uniqueidentifier = '70000000-0000-0000-0000-000000000002';
DECLARE @CustomerRoleId uniqueidentifier = '70000000-0000-0000-0000-000000000003';

DECLARE @UsersSelfCreatePermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000001';
DECLARE @UsersSelfReadPermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000002';
DECLARE @UsersSelfUpdatePermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000003';
DECLARE @UsersSelfActivatePermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000004';
DECLARE @UsersSelfDeletePermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000005';
DECLARE @PermissionsSelfReadPermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000006';
DECLARE @CategoriesReadPermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000007';
DECLARE @CategoriesReadAdminPermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000008';
DECLARE @CategoriesManagePermissionId uniqueidentifier = '71000000-0000-0000-0000-000000000009';

IF NOT EXISTS (SELECT 1 FROM [uam].[Roles] WHERE [Id] = @AdminRoleId)
BEGIN
    INSERT INTO [uam].[Roles] ([Id], [Code], [Name], [Description], [CreatedAt])
    VALUES
    (@AdminRoleId, N'admin', N'Admin', N'Administrator role', SYSDATETIMEOFFSET()),
    (@SellerRoleId, N'seller', N'Seller', N'Seller role', SYSDATETIMEOFFSET()),
    (@CustomerRoleId, N'customer', N'Customer', N'Customer role', SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [uam].[Permissions] WHERE [Id] = @UsersSelfCreatePermissionId)
BEGIN
    INSERT INTO [uam].[Permissions] ([Id], [Code], [Name], [Description], [CreatedAt])
    VALUES
    (@UsersSelfCreatePermissionId, N'users.self.create', N'Create self user', N'Create current user profile', SYSDATETIMEOFFSET()),
    (@UsersSelfReadPermissionId, N'users.self.read', N'Read self user', N'Read current user profile', SYSDATETIMEOFFSET()),
    (@UsersSelfUpdatePermissionId, N'users.self.update', N'Update self user', N'Update current user profile', SYSDATETIMEOFFSET()),
    (@UsersSelfActivatePermissionId, N'users.self.activate', N'Activate self user', N'Activate current user profile', SYSDATETIMEOFFSET()),
    (@UsersSelfDeletePermissionId, N'users.self.delete', N'Delete self user', N'Archive current user profile', SYSDATETIMEOFFSET()),
    (@PermissionsSelfReadPermissionId, N'permissions.self.read', N'Read self permissions', N'Read current user permissions', SYSDATETIMEOFFSET()),
    (@CategoriesReadPermissionId, N'categories.read', N'Read categories', N'Read public category data', SYSDATETIMEOFFSET()),
    (@CategoriesReadAdminPermissionId, N'categories.read.admin', N'Read admin categories', N'Read admin category data', SYSDATETIMEOFFSET()),
    (@CategoriesManagePermissionId, N'categories.manage', N'Manage categories', N'Create, update, delete categories', SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [uam].[RolePermissions] WHERE [RoleId] = @AdminRoleId)
BEGIN
    INSERT INTO [uam].[RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt])
    VALUES
    (NEWID(), @AdminRoleId, @UsersSelfCreatePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @UsersSelfReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @UsersSelfUpdatePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @UsersSelfActivatePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @UsersSelfDeletePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @PermissionsSelfReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @CategoriesReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @CategoriesReadAdminPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @AdminRoleId, @CategoriesManagePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @SellerRoleId, @UsersSelfReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @SellerRoleId, @UsersSelfUpdatePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @SellerRoleId, @UsersSelfDeletePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @SellerRoleId, @PermissionsSelfReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @SellerRoleId, @CategoriesReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @CustomerRoleId, @UsersSelfReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @CustomerRoleId, @UsersSelfUpdatePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @CustomerRoleId, @UsersSelfDeletePermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @CustomerRoleId, @PermissionsSelfReadPermissionId, SYSDATETIMEOFFSET()),
    (NEWID(), @CustomerRoleId, @CategoriesReadPermissionId, SYSDATETIMEOFFSET());
END
