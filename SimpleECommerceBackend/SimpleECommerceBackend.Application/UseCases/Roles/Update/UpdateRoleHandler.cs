using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Roles.Update;

[AutoConstructor]
public sealed partial class UpdateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand, UpdateRoleResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<UpdateRoleResult> Handle(
        UpdateRoleCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var role = await _roleRepository.FindByIdAsync(cmd.RoleId);

        if (role is null)
            throw new NotFoundException("Role", cmd.RoleId);

        role.SetName(cmd.Name);

        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateRoleResult
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}