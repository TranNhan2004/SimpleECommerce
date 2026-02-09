using MediatR;

namespace SimpleECommerceBackend.Application.UseCases.Roles.Delete;

public sealed record DeleteRoleCommand(Guid RoleId)
    : IRequest<DeleteRoleResult>;