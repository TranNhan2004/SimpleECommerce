namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for business rule violations.
///     Maps to HTTP 400 Bad Request.
/// </summary>
public class BusinessException : DomainException
{
    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, string code) : base(message)
    {
        Code = code;
    }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public string? Code { get; }
}