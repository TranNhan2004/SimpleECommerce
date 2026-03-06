using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IUserProfileRepository
{
    Task<IReadOnlyList<UserProfile>> FindAllAsync();
    Task<UserProfile?> FindByIdAsync(Guid id);
    UserProfile Add(UserProfile userProfile);
    UserProfile Update(UserProfile userProfile);
}