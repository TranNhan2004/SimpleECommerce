namespace SimpleECommerceBackend.Application.Interfaces.Services.Email;

public interface IEmailProvider
{
    public string BuildAccountVerificationEmail(string verificationUrl);
    public string BuildPasswordResetEmail(string passwordResetUrl);
}