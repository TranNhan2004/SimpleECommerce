SET NOCOUNT ON;

DECLARE @ForeignKeyDropSql nvarchar(max) = N'';
DECLARE @ForeignKeyCreateSql nvarchar(max) = N'';
DECLARE @TruncateSql nvarchar(max) = N'';

SELECT @ForeignKeyDropSql = STRING_AGG(
    N'ALTER TABLE ' + QUOTENAME(parent_schema.name) + N'.' + QUOTENAME(parent_table.name) + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';',
    CHAR(13) + CHAR(10)
)
FROM sys.foreign_keys fk
INNER JOIN sys.tables parent_table ON parent_table.object_id = fk.parent_object_id
INNER JOIN sys.schemas parent_schema ON parent_schema.schema_id = parent_table.schema_id
WHERE parent_schema.name IN (N'business', N'translation');

SELECT @ForeignKeyCreateSql = STRING_AGG(
    N'ALTER TABLE ' + QUOTENAME(parent_schema.name) + N'.' + QUOTENAME(parent_table.name) +
    N' WITH CHECK ADD CONSTRAINT ' + QUOTENAME(fk.name) +
    N' FOREIGN KEY (' + parent_columns.column_list + N') REFERENCES ' + QUOTENAME(referenced_schema.name) + N'.' + QUOTENAME(referenced_table.name) +
    N' (' + referenced_columns.column_list + N')' +
    CASE fk.delete_referential_action
        WHEN 1 THEN N' ON DELETE CASCADE'
        WHEN 2 THEN N' ON DELETE SET NULL'
        WHEN 3 THEN N' ON DELETE SET DEFAULT'
        ELSE N''
    END +
    CASE fk.update_referential_action
        WHEN 1 THEN N' ON UPDATE CASCADE'
        WHEN 2 THEN N' ON UPDATE SET NULL'
        WHEN 3 THEN N' ON UPDATE SET DEFAULT'
        ELSE N''
    END +
    N';' + CHAR(13) + CHAR(10) + N'ALTER TABLE ' + QUOTENAME(parent_schema.name) + N'.' + QUOTENAME(parent_table.name) + N' CHECK CONSTRAINT ' + QUOTENAME(fk.name) + N';',
    CHAR(13) + CHAR(10)
)
FROM sys.foreign_keys fk
INNER JOIN sys.tables parent_table ON parent_table.object_id = fk.parent_object_id
INNER JOIN sys.schemas parent_schema ON parent_schema.schema_id = parent_table.schema_id
INNER JOIN sys.tables referenced_table ON referenced_table.object_id = fk.referenced_object_id
INNER JOIN sys.schemas referenced_schema ON referenced_schema.schema_id = referenced_table.schema_id
CROSS APPLY
(
    SELECT STUFF(
    (
        SELECT N', ' + QUOTENAME(parent_column.name)
        FROM sys.foreign_key_columns fkc
        INNER JOIN sys.columns parent_column
            ON parent_column.object_id = fkc.parent_object_id
           AND parent_column.column_id = fkc.parent_column_id
        WHERE fkc.constraint_object_id = fk.object_id
        ORDER BY fkc.constraint_column_id
        FOR XML PATH(''), TYPE
    ).value('.', 'nvarchar(max)'), 1, 2, N'') AS column_list
) parent_columns
CROSS APPLY
(
    SELECT STUFF(
    (
        SELECT N', ' + QUOTENAME(referenced_column.name)
        FROM sys.foreign_key_columns fkc
        INNER JOIN sys.columns referenced_column
            ON referenced_column.object_id = fkc.referenced_object_id
           AND referenced_column.column_id = fkc.referenced_column_id
        WHERE fkc.constraint_object_id = fk.object_id
        ORDER BY fkc.constraint_column_id
        FOR XML PATH(''), TYPE
    ).value('.', 'nvarchar(max)'), 1, 2, N'') AS column_list
) referenced_columns
WHERE parent_schema.name IN (N'business', N'translation');

IF NULLIF(@ForeignKeyDropSql, N'') IS NOT NULL
    EXEC sp_executesql @ForeignKeyDropSql;

TRUNCATE TABLE [business].[CartItems];
TRUNCATE TABLE [business].[CustomerShippingAddresses];
TRUNCATE TABLE [business].[Inventories];
TRUNCATE TABLE [business].[Notifications];
TRUNCATE TABLE [business].[OrderItems];
TRUNCATE TABLE [business].[Payments];
TRUNCATE TABLE [business].[ReviewResponses];
TRUNCATE TABLE [business].[Reviews];
TRUNCATE TABLE [business].[ProductVariantImages];
TRUNCATE TABLE [business].[ProductVariantPrices];
TRUNCATE TABLE [business].[ProductVariants];
TRUNCATE TABLE [business].[Products];
TRUNCATE TABLE [business].[Carts];
TRUNCATE TABLE [business].[Orders];
TRUNCATE TABLE [business].[Categories];
TRUNCATE TABLE [business].[SellerWarehouses];
TRUNCATE TABLE [business].[SellerShops];
TRUNCATE TABLE [business].[UserProfiles];
TRUNCATE TABLE [translation].[Translations];

IF NULLIF(@ForeignKeyCreateSql, N'') IS NOT NULL
    EXEC sp_executesql @ForeignKeyCreateSql;
