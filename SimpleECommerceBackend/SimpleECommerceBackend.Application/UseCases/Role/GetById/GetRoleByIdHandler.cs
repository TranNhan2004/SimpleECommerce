using MediatR;
using SimpleECommerceBackend.Application.Results;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Role.GetById;

[AutoConstructor]
public sealed partial class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<GetRoleByIdResult>>
{
    private readonly IRoleRepository _roleRepository;

    public async Task<Result<GetRoleByIdResult>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var role = await _roleRepository.FindByIdAsync(request.RoleId);

        if (role is null)
            return Result<GetRoleByIdResult>.Fail(RoleErrors.NotFound);

        return Result<GetRoleByIdResult>.Ok(new GetRoleByIdResult
        {
            Id = role.Id,
            Name = role.Name
        });
    }
}