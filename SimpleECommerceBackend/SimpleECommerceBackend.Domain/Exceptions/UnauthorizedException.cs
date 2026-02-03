namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for authentication failures.
///     Maps to HTTP 401 Unauthorized.
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Authentication is required")
        : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}