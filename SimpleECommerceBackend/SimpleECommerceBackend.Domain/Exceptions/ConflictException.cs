namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for resource conflicts (duplicates, concurrent updates).
///     Maps to HTTP 409 Conflict.
/// </summary>
public class ConflictException : ExceptionBase
{
    public ConflictException(string errorCode, string? internalMessage = null, IReadOnlyDictionary<string, object?>? details = null)
        : base(errorCode, internalMessage, details)
    {
    }
}