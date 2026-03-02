using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public partial class UserProfileRepository : IUserProfileRepository
{
    private readonly AppDbContext _db;
    
    public async Task<IReadOnlyList<UserProfile>> FindAllAsync()
    {
        return await _db.UserProfiles.ToListAsync();
    }

    public async Task<UserProfile?> FindByIdAsync(Guid id)
    {
        return await _db.UserProfiles.FindAsync(id);
    }

    public async Task<UserProfile?> FindByCredentialIdAsync(Guid credentialId)
    {
        return await _db.UserProfiles
            .Where(up => up.CredentialId == credentialId)
            .FirstOrDefaultAsync();
    }

    public UserProfile Add(UserProfile userProfile)
    {
        _db.UserProfiles.Add(userProfile);
        return userProfile;
    }

    public UserProfile Update(UserProfile userProfile)
    {
        _db.UserProfiles.Update(userProfile);
        return userProfile;
    }
}