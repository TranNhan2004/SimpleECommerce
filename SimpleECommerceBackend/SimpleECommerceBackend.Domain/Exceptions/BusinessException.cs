namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for all domain-related exceptions
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}