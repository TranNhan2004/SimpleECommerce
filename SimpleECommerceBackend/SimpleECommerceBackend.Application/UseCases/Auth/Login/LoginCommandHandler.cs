using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;

public class LoginResult
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; } 
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public Role Role { get; init; }
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}

[AutoConstructor]
public partial class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        var credential = await _credentialRepository.FindByEmailAsync(request.Email) ??
                         throw new UnauthorizedException("Invalid email or password");
        
        if (credential.Status != CredentialStatus.Active)
            throw new ForbiddenException(
                resource: "Credential",
                action: "Login",
                message: "You are not authorized to access this resource."
            );

        if (!_passwordHasher.Verify(request.Password, credential.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");
        
        var userProfile = await _userProfileRepository.FindByCredentialIdAsync(credential.Id) ?? 
                          throw new UnauthorizedException("Invalid email or password");

        var accessToken = _jwtGenerator.GenerateAccessToken(credential.Email, credential.Role);
        var refreshToken = _jwtGenerator.GenerateRefreshToken(credential.Email, credential.Role);

        return new LoginResult
        {
            UserId = userProfile.Id,
            Email = credential.Email,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            NickName = userProfile.NickName,
            Sex = userProfile.Sex,
            BirthDate = userProfile.BirthDate,
            AvatarUrl = userProfile.AvatarUrl,
            CreatedAt = userProfile.CreatedAt,
            UpdatedAt = userProfile.UpdatedAt,
            Role = credential.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}