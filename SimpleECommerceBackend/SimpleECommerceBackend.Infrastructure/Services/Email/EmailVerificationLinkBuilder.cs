using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

public class EmailVerificationLinkBuilder : IEmailVerificationLinkBuilder
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmailVerificationOptions _options;

    public EmailVerificationLinkBuilder(
        IHttpContextAccessor httpContextAccessor,
        IOptions<EmailVerificationOptions> options
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public string BuildConfirmationUrl(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new BusinessException("Verification token is required");

        var normalizedPath = _options.ConfirmationPath;
        var encodedToken = Uri.EscapeDataString(token.Trim());

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request is not null)
        {
            var origin = $"{request.Scheme}://{request.Host}{request.PathBase}".TrimEnd('/');
            return $"{origin}{normalizedPath}?token={encodedToken}";
        }

        throw new BusinessException("Unable to build email confirmation URL");
    }

}