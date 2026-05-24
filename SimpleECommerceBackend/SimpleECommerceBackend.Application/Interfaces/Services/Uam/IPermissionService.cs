using SimpleECommerceBackend.Application.Interfaces.Services.Caching;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Uam;

public interface IPermissionService : ICacheConsumingService
{
    Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
