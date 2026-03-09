using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Commands;

[AutoConstructor]
public partial class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ConfirmEmailResult>
{
    private readonly IEmailVerificationRepository _emailVerificationRepository;
    private readonly IKeycloakAdminService _keycloakAdminService;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var normalizedToken = TokenUtils.NormalizeToken(request.Token);
        var tokenHash = TokenUtils.HashToken(normalizedToken);

        var emailVerification = await _emailVerificationRepository.FindByTokenHashAsync(tokenHash)
            ?? throw new NotFoundException("Email verification token is invalid or does not exist");

        if (emailVerification.IsConfirmed)
        {
            return new ConfirmEmailResult
            {
                Email = emailVerification.Email,
                Message = "Email is already verified",
                ConfirmedAt = emailVerification.ConfirmedAt!.Value,
                AlreadyConfirmed = true
            };
        }

        var now = DateTimeOffset.UtcNow;
        if (emailVerification.IsExpired(now))
            throw new BusinessException("Email verification token has expired");

        await _keycloakAdminService.MarkEmailAsVerifiedAsync(
            emailVerification.UserId.ToString(),
            cancellationToken
        );

        emailVerification.Confirm(now);
        _emailVerificationRepository.Update(emailVerification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfirmEmailResult
        {
            Email = emailVerification.Email,
            Message = "Email verified successfully",
            ConfirmedAt = now,
            AlreadyConfirmed = false
        };
    }


}