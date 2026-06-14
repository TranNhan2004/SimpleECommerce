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
    [CreatedAt] datetimeoffset NOT NULL,
    [CreatedById] uniqueidentifier NOT NULL,
    [UpdatedAt] datetimeoffset NULL,
    [UpdatedById] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [DeletedAt] datetimeoffset NULL,
    [DeletedById] uniqueidentifier NULL,
    CONSTRAINT [PK_Translations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Translations_Users_CreatedById] FOREIGN KEY ([CreatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Translations_Users_UpdatedById] FOREIGN KEY ([UpdatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Translations_Users_DeletedById] FOREIGN KEY ([DeletedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Translations_EntityName_FieldName_RowId_Locale]
    ON [translation].[Translations] ([EntityName], [FieldName], [RowId], [Locale]);
GO
