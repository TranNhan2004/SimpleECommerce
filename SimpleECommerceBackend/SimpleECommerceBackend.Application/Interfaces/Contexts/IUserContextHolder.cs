using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.Application.Interfaces.Contexts;

public interface IUserContextHolder
{
    IUserContext GetUserContext();
    IUserContext GetActiveUserContext();
    void ThrowIfNoActiveUserContext();
}
