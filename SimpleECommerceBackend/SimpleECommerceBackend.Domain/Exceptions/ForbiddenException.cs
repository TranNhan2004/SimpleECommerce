namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for authorization failures.
///     Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : ExceptionBase
{
    public ForbiddenException(string errorCode, string? internalMessage = null, IReadOnlyDictionary<string, object?>? details = null)
        : base(errorCode, internalMessage, details)
    {
    }
}