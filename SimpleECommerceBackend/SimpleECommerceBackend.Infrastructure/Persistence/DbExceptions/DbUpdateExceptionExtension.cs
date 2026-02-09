using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Persistence.DbExceptions;

public static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueConstraintViolation(this DbUpdateException ex)
    {
        return ex.InnerException switch
        {
            SqlException sql => sql.Number is 2601 or 2627, // SQL Server
            _ => false
        };
    }

    public static ConflictException ToConflictException(
        this DbUpdateException ex,
        DbContext dbContext)
    {
        var entry = ex.Entries.FirstOrDefault();
        if (entry is null)
            return new ConflictException("Unknown", "", "", "");

        var entityName = entry.Metadata.ClrType.Name;

        var uniqueIndex = entry.Metadata
            .GetIndexes()
            .FirstOrDefault(i => i.IsUnique);

        if (uniqueIndex is null)
            return new ConflictException(entityName, "", "", "");

        var property = uniqueIndex.Properties.First();
        var value = entry.CurrentValues[property.Name]!;

        return new ConflictException(
            entityName,
            property.Name,
            value,
            $"{property.Name} already exists."
        );
    }
}