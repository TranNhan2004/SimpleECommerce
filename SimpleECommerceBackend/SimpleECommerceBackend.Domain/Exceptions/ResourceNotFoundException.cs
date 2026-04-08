namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for not existed resource.
///     Maps to HTTP 404 Not Found.
/// </summary>
public class ResourceNotFoundException : ExceptionBase
{
    public ResourceNotFoundException(string errorCode, string? internalMessage = null, IReadOnlyDictionary<string, object?>? details = null)
        : base(errorCode, internalMessage, details)
    {
    }
}