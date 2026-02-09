using MediatR;

namespace SimpleECommerceBackend.Application.UseCases.Roles.Update;

public sealed record UpdateRoleCommand(
    Guid RoleId,
    string Name
) : IRequest<UpdateRoleResult>;