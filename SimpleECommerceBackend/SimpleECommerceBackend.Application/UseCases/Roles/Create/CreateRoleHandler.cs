using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Application.UseCases.Roles.Create;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

[AutoConstructor]
public sealed partial class CreateRoleCommandHandler
    : IRequestHandler<CreateRoleCommand, CreateRoleResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<CreateRoleResult> Handle(
        CreateRoleCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var role = Role.Create(cmd.Name);

        _roleRepository.Add(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRoleResult
        {
            RoleId = role.Id
        };
    }
}