CREATE SCHEMA [translation];
GO

CREATE TABLE [translation].[Translations]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [EntityName] NVARCHAR(128) NOT NULL,
    [FieldName] NVARCHAR(128) NOT NULL,
    [RowId] UNIQUEIDENTIFIER NOT NULL,
    [Locale] NVARCHAR(16) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_Translations] PRIMARY KEY ([Id]),
    CONSTRAINT [UX_Translations_Entity_Field_Row_Locale]
        UNIQUE ([EntityName], [FieldName], [RowId], [Locale])
);
GO
