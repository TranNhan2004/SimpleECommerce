namespace SimpleECommerceBackend.Application.Interfaces.Services.Email;

public interface IEmailVerificationLinkBuilder
{
    string BuildConfirmationUrl(string token);
}