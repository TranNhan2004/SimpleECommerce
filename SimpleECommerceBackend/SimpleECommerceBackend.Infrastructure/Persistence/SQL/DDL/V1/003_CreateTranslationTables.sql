IF SCHEMA_ID(N'translation') IS NULL
    EXEC(N'CREATE SCHEMA translation');
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
