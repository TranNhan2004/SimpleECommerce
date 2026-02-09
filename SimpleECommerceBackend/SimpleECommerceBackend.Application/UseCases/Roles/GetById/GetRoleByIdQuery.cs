using MediatR;

namespace SimpleECommerceBackend.Application.UseCases.Roles.GetById;

public sealed record GetRoleByIdQuery(Guid RoleId) : IRequest<GetRoleByIdResult>;