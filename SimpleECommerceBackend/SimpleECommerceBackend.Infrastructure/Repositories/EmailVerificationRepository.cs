using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories;

[AutoConstructor]
public partial class EmailVerificationRepository : IEmailVerificationRepository
{
    private readonly AppDbContext _db;

    public async Task<EmailVerification?> FindByTokenHashAsync(string tokenHash)
    {
        return await _db.EmailVerifications
            .FirstOrDefaultAsync(emailVerification => emailVerification.TokenHash == tokenHash);
    }

    public EmailVerification Add(EmailVerification emailVerification)
    {
        _db.EmailVerifications.Add(emailVerification);
        return emailVerification;
    }

    public EmailVerification Update(EmailVerification emailVerification)
    {
        _db.EmailVerifications.Update(emailVerification);
        return emailVerification;
    }
}