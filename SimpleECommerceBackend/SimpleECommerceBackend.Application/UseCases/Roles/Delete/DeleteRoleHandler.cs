using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Roles.Delete;

[AutoConstructor]
public sealed partial class DeleteRoleCommandHandler
    : IRequestHandler<DeleteRoleCommand, DeleteRoleResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<DeleteRoleResult> Handle(
        DeleteRoleCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var role = await _roleRepository.FindByIdAsync(cmd.RoleId);

        if (role is null)
            throw new NotFoundException("Role", cmd.RoleId);

        _roleRepository.Delete(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeleteRoleResult
        {
            RoleId = role.Id
        };
    }
}