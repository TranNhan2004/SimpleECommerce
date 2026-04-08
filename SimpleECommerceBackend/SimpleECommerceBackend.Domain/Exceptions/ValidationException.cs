namespace SimpleECommerceBackend.Domain.Exceptions;

public class ValidationException : ExceptionBase
{
    public ValidationException(string errorCode, string? internalMessage = null, IReadOnlyDictionary<string, object?>? details = null)
        : base(errorCode, internalMessage, details)
    {
    }
}
