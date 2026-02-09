using MediatR;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Roles.GetAll;

[AutoConstructor]
public sealed partial class GetAllRolesQueryHandler :
    IRequestHandler<GetAllRolesQuery, GetAllRolesResult>
{
    private readonly IRoleRepository _roleRepository;

    public async Task<GetAllRolesResult> Handle(
        GetAllRolesQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var roles = await _roleRepository.FindAllAsync();

        return new GetAllRolesResult
        {
            Roles = roles.Select(r => new GetAllRolesResult.RoleItem
            {
                Id = r.Id,
                Name = r.Name
            })
        };
    }
}