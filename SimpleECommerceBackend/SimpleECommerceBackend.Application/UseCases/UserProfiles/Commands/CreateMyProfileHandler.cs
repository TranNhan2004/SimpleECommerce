using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

[AutoConstructor]
public partial class CreateMyProfileHandler : IUseCaseHandler<CreateMyProfileCommand, CreateMyProfileResult>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<CreateMyProfileResult> HandleAsync(
        CreateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var userContext = _userContextHolder.GetUserContext();

        var userProfile = UserProfile.Create(
            userContext.Id,
            userContext.Email,
            request.FirstName,
            request.LastName,
            request.NickName,
            SexUtils.Parse(request.Sex),
            request.BirthDate,
            null
        );

        var userProfileCreated = _userProfileService.CreateUserProfile(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateMyProfileResult
        {
            FirstName = userProfileCreated.FirstName,
            LastName = userProfileCreated.LastName,
            NickName = userProfileCreated.NickName,
            Sex = SexUtils.ToName(userProfileCreated.Sex),
            BirthDate = userProfileCreated.BirthDate,
        };
    }
}