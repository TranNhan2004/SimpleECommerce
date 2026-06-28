SET NOCOUNT ON;

DECLARE @ForeignKeyDropSql nvarchar(max) = N'';

SELECT @ForeignKeyDropSql = STRING_AGG(
    N'ALTER TABLE ' + QUOTENAME(parent_schema.name) + N'.' + QUOTENAME(parent_table.name) +
    N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';',
    CHAR(13) + CHAR(10)
)
FROM sys.foreign_keys fk
INNER JOIN sys.tables parent_table
    ON parent_table.object_id = fk.parent_object_id
INNER JOIN sys.schemas parent_schema
    ON parent_schema.schema_id = parent_table.schema_id
WHERE parent_schema.name IN (N'business', N'uam', N'translation', N'audit_tracking');

IF NULLIF(@ForeignKeyDropSql, N'') IS NOT NULL
    EXEC sp_executesql @ForeignKeyDropSql;

-- Tables defined in DDL/V1 business schema
DROP TABLE IF EXISTS [business].[CartItems];
DROP TABLE IF EXISTS [business].[CustomerShippingAddresses];
DROP TABLE IF EXISTS [business].[Inventories];
DROP TABLE IF EXISTS [business].[Notifications];
DROP TABLE IF EXISTS [business].[OrderItems];
DROP TABLE IF EXISTS [business].[Payments];
DROP TABLE IF EXISTS [business].[ReviewResponses];
DROP TABLE IF EXISTS [business].[Reviews];
DROP TABLE IF EXISTS [business].[ProductVariantImages];
DROP TABLE IF EXISTS [business].[ProductVariantPrices];
DROP TABLE IF EXISTS [business].[ProductVariants];
DROP TABLE IF EXISTS [business].[Products];
DROP TABLE IF EXISTS [business].[Carts];
DROP TABLE IF EXISTS [business].[Orders];
DROP TABLE IF EXISTS [business].[SellerWarehouses];
DROP TABLE IF EXISTS [business].[SellerShops];
DROP TABLE IF EXISTS [business].[Categories];

-- Tables defined in DDL/V1 auxiliary schemas
DROP TABLE IF EXISTS [translation].[Translations];
DROP TABLE IF EXISTS [audit_tracking].[Audits];

-- Tables defined in DDL/V1 uam schema
DROP TABLE IF EXISTS [uam].[RolePermissions];
DROP TABLE IF EXISTS [uam].[UserRoles];
DROP TABLE IF EXISTS [uam].[Permissions];
DROP TABLE IF EXISTS [uam].[Roles];
DROP TABLE IF EXISTS [uam].[Users];
