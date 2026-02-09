namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for resource conflicts (duplicates, concurrent updates).
///     Maps to HTTP 409 Conflict.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string entityName, string conflictingField, object conflictingValue, string message)
        : base(message)
    {
        EntityName = entityName;
        ConflictingField = conflictingField;
        ConflictingValue = conflictingValue;
    }

    public string? EntityName { get; }
    public string? ConflictingField { get; }
    public object? ConflictingValue { get; }
}