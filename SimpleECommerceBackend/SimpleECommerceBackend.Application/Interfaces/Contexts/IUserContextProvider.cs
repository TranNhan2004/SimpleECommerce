using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.Application.Interfaces.Contexts;

public interface ICurrentUserContextProvider
{
    ICurrentUserContext GetUserContext();
    Task<ICurrentUserContext> GetUserContextAsync(CancellationToken cancellationToken = default);
}
