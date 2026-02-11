using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

public interface ICredentialRepository
{
    Task<Credential?> FindByIdAsync(Guid id);
    Task<Credential?> FindByEmailAsync(string email);
    Credential Add(Credential credential);
    Credential Update(Credential credential);
}