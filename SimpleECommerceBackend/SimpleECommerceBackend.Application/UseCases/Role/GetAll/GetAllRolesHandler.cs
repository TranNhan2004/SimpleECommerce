using MediatR;
using SimpleECommerceBackend.Application.Results;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Role.GetAll;

[AutoConstructor]
public sealed partial class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<GetAllRolesResult>>
{
    private readonly IRoleRepository _roleRepository;

    public async Task<Result<GetAllRolesResult>> Handle(
        GetAllRolesQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var roles = await _roleRepository.FindAllAsync();

        return Result<GetAllRolesResult>.Ok(new GetAllRolesResult
        {
            Roles = roles.Select(r => new GetAllRolesResult.RoleItem
            {
                Id = r.Id,
                Name = r.Name
            })
        });
    }
}