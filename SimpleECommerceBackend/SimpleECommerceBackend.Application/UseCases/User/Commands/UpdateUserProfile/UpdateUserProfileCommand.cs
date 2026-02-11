using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Interfaces.Time;

namespace SimpleECommerceBackend.Application.UseCases.User.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? NickName,
    Sex Sex,
    DateOnly BirthDate,
    string? AvatarUrl
) : IRequest;

[AutoConstructor]
public partial class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.FindByIdAsync(request.UserId);
        if (profile is null)
            throw new KeyNotFoundException($"User profile {request.UserId} not found");

        profile.SetFirstName(request.FirstName);
        profile.SetLastName(request.LastName);
        profile.SetNickName(request.NickName);
        profile.SetSex(request.Sex);
        profile.SetBirthDate(request.BirthDate, _clock);
        profile.SetAvatarUrl(request.AvatarUrl);

        _userProfileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
