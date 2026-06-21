namespace SimpleECommerceBackend.Application.Interfaces.Security;

/// <summary>
/// Represents the authenticated application user for the current HTTP request.
/// Use this in request handlers that require a logged-in user.
/// </summary>
public interface ICurrentUserContext
{
    Guid Id { get; }
    Guid KeycloakSubjectId { get; }
    string Email { get; }
}
