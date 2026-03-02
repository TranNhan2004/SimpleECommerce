using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Interfaces.Security;

public interface IUserContext
{
    public Guid Id { get; }
    public string Email { get; }
    public Role Role { get; }
}