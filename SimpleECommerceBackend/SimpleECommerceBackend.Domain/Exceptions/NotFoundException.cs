namespace SimpleECommerceBackend.Domain.Exceptions;

/// <summary>
///     Exception for not existed resource.
///     Maps to HTTP 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object entityId)
        : base($"{entityName} with identifier '{entityId}' was not found")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public NotFoundException(string message) : base(message)
    {
        EntityName = string.Empty;
        EntityId = string.Empty;
    }

    public string EntityName { get; }
    public object EntityId { get; }
}