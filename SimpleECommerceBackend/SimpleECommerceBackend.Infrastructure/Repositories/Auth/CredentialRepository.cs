using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Auth;

[AutoConstructor]
public sealed partial class CredentialRepository : ICredentialRepository
{
    private readonly AppDbContext _db;

    public async Task<Credential?> FindByIdAsync(Guid id)
    {
        return await _db.Credentials.FindAsync(id);
    }

    public async Task<Credential?> FindByEmailAsync(string email)
    {
        return await _db.Credentials
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public Credential Add(Credential credential)
    {
        _db.Credentials.Add(credential);
        return credential;
    }

    public Credential Update(Credential credential)
    {
        _db.Credentials.Update(credential);
        return credential;
    }
}