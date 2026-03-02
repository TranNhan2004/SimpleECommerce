using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Auth.VerifyAccount;

public record VerifyAccountCommand(
    string Token
) : IRequest;

[AutoConstructor]
public partial class VerifyAccountCommandHandler : IRequestHandler<VerifyAccountCommand>
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task Handle(VerifyAccountCommand request, CancellationToken cancellationToken = default)
    {
        var claimsInfo = _jwtGenerator.ValidateToken(request.Token);

        if (claimsInfo.TokenType != TokenType.AccountVerificationToken)
            throw new UnauthorizedException("Invalid token");
        
        var credential = await _credentialRepository.FindByEmailAsync(claimsInfo.Email)
            ?? throw new UnauthorizedException("You are not registered");
        
        credential.Activate();
        _credentialRepository.Update(credential);

        var userProfile = UserProfile.Create(
            credential.Id,
            credential.Email,
            "Your First Name",
            "Your Last Name",
            null,
            Sex.Other,
            AgeUtils.CreateRandomBirthDate(UserProfileConstants.MinAge, UserProfileConstants.MaxAge),
            null
        );
        _userProfileRepository.Add(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}