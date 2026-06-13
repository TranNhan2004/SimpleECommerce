IF SCHEMA_ID(N'audit_tracking') IS NULL
    EXEC(N'CREATE SCHEMA audit_tracking');
GO

CREATE TABLE [audit_tracking].[Audits]
(
    [Id] uniqueidentifier NOT NULL,
    [EntityName] nvarchar(255) NOT NULL,
    [EntityId] uniqueidentifier NOT NULL,
    [OperationType] int NOT NULL,
    [TraceId] nvarchar(127) NOT NULL,
    [IpAddress] nvarchar(45) NOT NULL,
    [OldValues] nvarchar(max) NULL,
    [NewValues] nvarchar(max) NULL,
    [AuditedById] uniqueidentifier NOT NULL,
    [AuditedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Audits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Audits_Users_AuditedById] FOREIGN KEY ([AuditedById]) REFERENCES [uam].[Users] ([Id]) 
    ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Audits_EntityName_EntityId]
    ON [audit_tracking].[Audits] ([EntityName], [EntityId]);
GO

CREATE INDEX [IX_Audits_AuditedById]
    ON [audit_tracking].[Audits] ([AuditedById]);
GO

CREATE INDEX [IX_Audits_AuditedAt]
    ON [audit_tracking].[Audits] ([AuditedAt]);
GO

CREATE INDEX [IX_Audits_TraceId]
    ON [audit_tracking].[Audits] ([TraceId]);
GO

CREATE INDEX [IX_Audits_IpAddress]
    ON [audit_tracking].[Audits] ([IpAddress]);
GO
