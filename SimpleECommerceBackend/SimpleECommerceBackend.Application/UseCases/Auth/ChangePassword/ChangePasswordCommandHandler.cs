using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.UseCases.Auth.ChangePassword;

public record ChangePasswordCommand(
    string Email,
    string OldPassword,
    string NewPassword,
    string ConfirmedNewPassword
) : IRequest;

[AutoConstructor]
public partial class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken = default)
    {
        var credential = await _credentialRepository.FindByEmailAsync(request.Email) ??
                         throw new NotFoundException("User not found");

        if (_passwordHasher.Verify(request.OldPassword, credential.PasswordHash))
            throw new BusinessException("Old password is incorrect");
        
        if (request.OldPassword == request.NewPassword)
            throw new BusinessException("New password must be different from old password");
        
        if (request.NewPassword != request.ConfirmedNewPassword) 
            throw new BusinessException("Passwords don't match");
        
        credential.SetPasswordHash(_passwordHasher.Hash(request.NewPassword));
        _credentialRepository.Update(credential);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}