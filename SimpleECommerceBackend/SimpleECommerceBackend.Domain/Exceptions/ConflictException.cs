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

    public ConflictException(string message, string conflictingField, object conflictingValue)
        : base(message)
    {
        ConflictingField = conflictingField;
        ConflictingValue = conflictingValue;
    }

    public string? ConflictingField { get; }
    public object? ConflictingValue { get; }
}