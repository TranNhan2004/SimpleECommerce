using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class CreateMyProfileHandler : IUseCaseHandler<CreateMyProfileCommand, CreateMyProfileResult>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMyProfileHandler(
        IUserContextHolder userContextHolder,
        IUserProfileService userProfileService,
        IUnitOfWork unitOfWork
    )
    {
        _userContextHolder = userContextHolder;
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateMyProfileResult> HandleAsync(
        CreateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var userContext = _userContextHolder.GetUserContext();

        var userProfile = new UserProfile
        {
            Id = userContext.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NickName = request.NickName,
            Email = userContext.Email,
            Sex = request.Sex,
            BirthDate = request.BirthDate,
            Status = UserStatus.Active,
            AvatarUrl = null
        };

        var userProfileCreated = _userProfileService.CreateUserProfile(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateMyProfileResult.FromEntity(userProfileCreated);
    }
}
