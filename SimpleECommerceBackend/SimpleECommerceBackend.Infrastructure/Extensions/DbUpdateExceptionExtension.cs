using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Extensions;

public static class DbUpdateConflictExceptionExtensions
{
    public static bool IsUniqueConstraintViolation(this DbUpdateException ex)
    {
        return ex.InnerException switch
        {
            SqlException sql => sql.Number == 2601 || sql.Number == 2627, // SQL Server
            _ => false
        };
    }

    public static ConflictException ToConflictException(
        this DbUpdateException ex,
        DbContext dbContext
    )
    {
        var entry = ex.Entries.FirstOrDefault();
        if (entry is null)
            return new ConflictException(UniqueErrorCode.UnknownError);

        var entityName = entry.Metadata.ClrType.Name;

        var uniqueIndex = entry.Metadata
            .GetIndexes()
            .FirstOrDefault(i => i.IsUnique);

        if (uniqueIndex is null)
            return new ConflictException(UniqueErrorCode.HasNoIndex);

        var property = uniqueIndex.Properties.First();
        var value = entry.CurrentValues[property.Name]!;

        return new ConflictException(
            UniqueErrorCode.DuplicateValue,
            "Duplicate value for unique constraint",
            new Dictionary<string, object?>
            {
                { "entity", entityName },
                { "field", property.Name },
                { "value", value }
            }
        );
    }
}