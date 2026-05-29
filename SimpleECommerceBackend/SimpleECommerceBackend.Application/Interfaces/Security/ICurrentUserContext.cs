namespace SimpleECommerceBackend.Application.Interfaces.Security;

public interface ICurrentUserContext
{
    public Guid Id { get; }
    public Guid KeycloakSubjectId { get; }
    public string Email { get; }
}
