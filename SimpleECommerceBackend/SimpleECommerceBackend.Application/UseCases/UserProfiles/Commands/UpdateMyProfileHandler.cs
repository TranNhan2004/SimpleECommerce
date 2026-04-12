using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

[AutoConstructor]
public partial class UpdateMyProfileHandler : IUseCaseHandler<UpdateMyProfileCommand, UpdateMyProfileResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;

    public async Task<UpdateMyProfileResult> HandleAsync(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var userProfile = await _userProfileService.GetByIdForUpdateAsync(currentUser.Id);

        if (request.FirstName is not null)
            userProfile.SetFirstName(request.FirstName);
        if (request.LastName is not null)
            userProfile.SetLastName(request.LastName);
        if (request.Sex is not null)
            userProfile.SetSex(SexUtils.Parse(request.Sex));
        if (request.NickName is not null)
            userProfile.SetNickName(request.NickName);
        if (request.BirthDate.HasValue)
            userProfile.SetBirthDate(request.BirthDate.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _userProfileService.InvalidateCacheAsync(userProfile.Id);
        return new UpdateMyProfileResult
        {
            Id = userProfile.Id,
            Email = userProfile.Email,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            NickName = userProfile.NickName,
            Sex = SexUtils.ToName(userProfile.Sex),
            Status = UserStatusUtils.ToName(userProfile.Status),
            BirthDate = userProfile.BirthDate
        };
    }
}