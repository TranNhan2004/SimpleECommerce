namespace SimpleECommerceBackend.Application.Interfaces.Services.Uam;

public interface IPermissionService
{
    Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
