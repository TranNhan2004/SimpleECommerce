namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for validation errors (input validation, format, required fields).
///     Maps to HTTP 422 Unprocessable Entity.
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            ["general"] = [message]
        };
    }

    public ValidationException(string field, string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            [field] = [message]
        };
    }

    public ValidationException(Dictionary<string, string[]> errors) : base("One or more validation errors occurred")
    {
        Errors = errors;
    }

    public ValidationException(string message, Dictionary<string, string[]> errors) : base(message)
    {
        Errors = errors;
    }

    public Dictionary<string, string[]> Errors { get; }
}