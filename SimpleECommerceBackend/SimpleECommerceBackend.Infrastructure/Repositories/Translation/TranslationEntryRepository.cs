using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Translation;
using SimpleECommerceBackend.Domain.Entities.Translation;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Translation;

[AutoConstructor]
public partial class TranslationEntryRepository : ITranslationEntryRepository
{
    private readonly IConfiguration _configuration;

    public async Task<TranslationEntry?> FindAsync(
        string entityName,
        string fieldName,
        Guid rowId,
        string locale,
        CancellationToken cancellationToken = default
    )
    {
        const string sql = """
                           SELECT TOP (1)
                               [Id],
                               [EntityName],
                               [FieldName],
                               [RowId],
                               [Locale],
                               [Value]
                           FROM [translation].[Translations]
                           WHERE [EntityName] = @EntityName
                             AND [FieldName] = @FieldName
                             AND [RowId] = @RowId
                             AND [Locale] = @Locale;
                           """;

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new
            {
                EntityName = entityName,
                FieldName = fieldName,
                RowId = rowId,
                Locale = locale.ToLowerInvariant()
            },
            cancellationToken: cancellationToken
        );

        var record = await connection.QuerySingleOrDefaultAsync<TranslationRecord>(command);
        return record is null
            ? null
            : new TranslationEntry(
                record.Id,
                record.EntityName,
                record.FieldName,
                record.RowId,
                record.Locale,
                record.Value
            );
    }

    public async Task UpsertAsync(
        TranslationEntry entry,
        CancellationToken cancellationToken = default
    )
    {
        const string sql = """
                           SET XACT_ABORT ON;
                           BEGIN TRANSACTION;

                           UPDATE [translation].[Translations] WITH (UPDLOCK, SERIALIZABLE)
                           SET [Value] = @Value
                           WHERE [EntityName] = @EntityName
                             AND [FieldName] = @FieldName
                             AND [RowId] = @RowId
                             AND [Locale] = @Locale;

                           IF @@ROWCOUNT = 0
                           BEGIN
                               INSERT INTO [translation].[Translations] ([Id], [EntityName], [FieldName], [RowId], [Locale], [Value])
                               VALUES (@Id, @EntityName, @FieldName, @RowId, @Locale, @Value);
                           END

                           COMMIT TRANSACTION;
                           """;

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new
            {
                entry.Id,
                entry.EntityName,
                entry.FieldName,
                entry.RowId,
                entry.Locale,
                entry.Value
            },
            cancellationToken: cancellationToken
        );

        await connection.ExecuteAsync(command);
    }

    private SqlConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' is not configured.");

        return new SqlConnection(connectionString);
    }

    private sealed class TranslationRecord
    {
        public Guid Id { get; init; }
        public string EntityName { get; init; } = string.Empty;
        public string FieldName { get; init; } = string.Empty;
        public Guid RowId { get; init; }
        public string Locale { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
    }
}