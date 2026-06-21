IF SCHEMA_ID(N'uam') IS NULL
    EXEC(N'CREATE SCHEMA uam');
GO

CREATE TABLE [uam].[Users]
(
    [Id] uniqueidentifier NOT NULL,
    [KeycloakSubjectId] uniqueidentifier NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(64) NOT NULL,
    [LastName] nvarchar(128) NOT NULL,
    [NickName] nvarchar(128) NULL,
    [Sex] nvarchar(16) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [BirthDate] date NOT NULL,
    [AvatarUrl] nvarchar(max) NULL,
    [LastLoginAt] datetimeoffset NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [CreatedById] uniqueidentifier NULL,
    [UpdatedAt] datetimeoffset NULL,
    [UpdatedById] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0, 
    [DeletedAt] datetimeoffset NULL,
    [DeletedById] uniqueidentifier NULL,
    CONSTRAINT [FK_Users_CreatedById] FOREIGN KEY ([CreatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Users_UpdatedById] FOREIGN KEY ([UpdatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Users_DeletedById] FOREIGN KEY ([DeletedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Users_KeycloakSubjectId]
    ON [uam].[Users] ([KeycloakSubjectId]);
GO

CREATE INDEX [IX_Users_Email]
    ON [uam].[Users] ([Email])
GO

CREATE TABLE [uam].[Roles]
(
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(64) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Description] nvarchar(512) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [CreatedById] uniqueidentifier NULL,
    [UpdatedAt] datetimeoffset NULL,
    [UpdatedById] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [DeletedAt] datetimeoffset NULL,
    [DeletedById] uniqueidentifier NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Roles_CreatedById] FOREIGN KEY ([CreatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Roles_UpdatedById] FOREIGN KEY ([UpdatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Roles_DeletedById] FOREIGN KEY ([DeletedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION  
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
    [CreatedById] uniqueidentifier NULL,
    [UpdatedAt] datetimeoffset NULL,
    [UpdatedById] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [DeletedAt] datetimeoffset NULL,
    [DeletedById] uniqueidentifier NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Permissions_CreatedById] FOREIGN KEY ([CreatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Permissions_UpdatedById] FOREIGN KEY ([UpdatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_Permissions_DeletedById] FOREIGN KEY ([DeletedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION
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
    [CreatedById] uniqueidentifier NULL,
    [UpdatedAt] datetimeoffset NULL,
    [UpdatedById] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [DeletedAt] datetimeoffset NULL,
    [DeletedById] uniqueidentifier NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRoles_CreatedById] FOREIGN KEY ([CreatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRoles_UpdatedById] FOREIGN KEY ([UpdatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRoles_DeletedById] FOREIGN KEY ([DeletedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [uam].[Roles] ([Id])
        ON DELETE NO ACTION
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
    [CreatedById] uniqueidentifier NULL,
    [UpdatedAt] datetimeoffset NULL,
    [UpdatedById] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [DeletedAt] datetimeoffset NULL,
    [DeletedById] uniqueidentifier NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [uam].[Roles] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId])
        REFERENCES [uam].[Permissions] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_RolePermissions_CreatedById] FOREIGN KEY ([CreatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_RolePermissions_UpdatedById] FOREIGN KEY ([UpdatedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION,
    CONSTRAINT [FK_RolePermissions_DeletedById] FOREIGN KEY ([DeletedById])
        REFERENCES [uam].[Users] ([Id])
        ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_RolePermissions_RoleId_PermissionId]
    ON [uam].[RolePermissions] ([RoleId], [PermissionId]);
GO
