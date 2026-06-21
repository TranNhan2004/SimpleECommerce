using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.Uam;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Users.Commands;

public class CreateMeHandler : IUseCaseHandler<CreateMeCommand, CreateMeResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUserService _userService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMeHandler(
        Serilog.ILogger logger,
        ICurrentUserContext currentUserContext,
        IUserService userService,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _currentUserContext = currentUserContext;
        _userService = userService;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateMeResult> HandleAsync(
        CreateMeCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = new User
        {
            Id = _currentUserContext.Id,
            KeycloakSubjectId = _currentUserContext.KeycloakSubjectId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NickName = request.NickName,
            Email = _currentUserContext.Email,
            Sex = request.Sex,
            BirthDate = request.BirthDate,
            Status = UserStatus.Active,
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

        return CreateMeResult.FromEntity(userCreated);
    }
}
