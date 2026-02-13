namespace SimpleECommerceBackend.Application.Interfaces.Services.Email;

public interface IEmailService
{
    ValueTask SendAsync(string to, string subject, string body);
}