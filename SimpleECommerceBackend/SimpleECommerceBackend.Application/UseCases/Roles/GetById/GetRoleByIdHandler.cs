using MediatR;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Roles.GetById;

[AutoConstructor]
public sealed partial class GetRoleByIdQueryHandler :
    IRequestHandler<GetRoleByIdQuery, GetRoleByIdResult>
{
    private readonly IRoleRepository _roleRepository;

    public async Task<GetRoleByIdResult> Handle(
        GetRoleByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var role = await _roleRepository.FindByIdAsync(query.RoleId);

        if (role is null)
            throw new NotFoundException("Role", query.RoleId);

        return new GetRoleByIdResult
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}