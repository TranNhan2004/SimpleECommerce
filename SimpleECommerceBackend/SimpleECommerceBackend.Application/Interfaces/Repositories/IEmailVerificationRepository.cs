using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IEmailVerificationRepository
{
    Task<EmailVerification?> FindByTokenHashAsync(string tokenHash);
    EmailVerification Add(EmailVerification emailVerification);
    EmailVerification Update(EmailVerification emailVerification);
}