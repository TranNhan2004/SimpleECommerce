using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

[AutoConstructor]
public partial class UpdateCommandHandler : IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;

    public async Task<UpdateUserProfileResult> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var userProfile = await _userProfileService.GetByIdForUpdateAsync(currentUser.Id);

        userProfile.SetFirstName(request.FirstName);
        userProfile.SetLastName(request.LastName);
        userProfile.SetNickName(request.NickName);
        userProfile.SetSex(SexUtils.Parse(request.Sex));
        userProfile.SetBirthDate(request.BirthDate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UpdateUserProfileResult();
    }
}
