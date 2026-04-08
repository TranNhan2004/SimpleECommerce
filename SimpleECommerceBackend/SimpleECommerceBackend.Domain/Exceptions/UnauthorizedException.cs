namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for authentication failures.
///     Maps to HTTP 401 Unauthorized.
/// </summary>
public class UnauthorizedException : ExceptionBase
{
    public UnauthorizedException(string errorCode, string? internalMessage = null, IReadOnlyDictionary<string, object?>? details = null)
        : base(errorCode, internalMessage, details)
    {
    }
}