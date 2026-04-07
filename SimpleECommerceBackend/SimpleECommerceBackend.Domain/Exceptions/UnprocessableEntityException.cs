using System.Net;

namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///    Exception for unprocessable entity, e.g. mismatch fields.
/// </summary>
public class UnprocessableEntityException : ExceptionBase
{
    public UnprocessableEntityException(string errorCode, string? internalMessage = null, IReadOnlyDictionary<string, object?>? details = null)
        : base(errorCode, internalMessage, details)
    {
    }
}