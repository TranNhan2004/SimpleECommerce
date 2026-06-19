using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.Uam;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

public class CreateMyProfileHandler : IUseCaseHandler<CreateMyProfileCommand, CreateMyProfileResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly ICurrentUserContextProvider _userContextHolder;
    private readonly IUserService _userService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMyProfileHandler(
        Serilog.ILogger logger,
        ICurrentUserContextProvider userContextHolder,
        IUserService userService,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _userContextHolder = userContextHolder;
        _userService = userService;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateMyProfileResult> HandleAsync(
        CreateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var userContext = _userContextHolder.GetUserContext();

        var user = new User
        {
            Id = userContext.Id,
            KeycloakSubjectId = userContext.KeycloakSubjectId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NickName = request.NickName,
            Email = userContext.Email,
            Sex = request.Sex,
            BirthDate = request.BirthDate,
            IsActive = true,
            AvatarUrl = null
        };

        var userCreated = _userService.CreateUser(user);
        var customerRole = await _roleRepository.FindByCodeAsync(RoleCodes.Customer, true)
            ?? throw new ResourceNotFoundException(
                UamErrorCodes.RoleNotFoundByCode,
                $"Role with code '{RoleCodes.Customer}' not found."
            );

        _userRoleRepository.Add(
            new UserRole
            {
                Id = UuidUtils.CreateV7(),
                UserId = userCreated.Id,
                RoleId = customerRole.Id
            }
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateMyProfileResult.FromEntity(userCreated);
    }
}
