using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IUserProfileRepository
{
    Task<IReadOnlyList<UserProfile>> FindAllAsync();
    Task<UserProfile?> FindByIdAsync(Guid id);
    Task<UserProfile?> FindByCredentialIdAsync(Guid credentialId); 
    UserProfile Add(UserProfile userProfile);
    UserProfile Update(UserProfile userProfile);
}