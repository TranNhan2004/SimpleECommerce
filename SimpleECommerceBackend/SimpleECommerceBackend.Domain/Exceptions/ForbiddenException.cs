namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for authorization failures.
///     Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You don't have permission to perform this action")
        : base(message)
    {
    }

    public ForbiddenException(string message, string resource, string action)
        : base(message)
    {
        Resource = resource;
        Action = action;
    }

    public string? Resource { get; }
    public string? Action { get; }
}