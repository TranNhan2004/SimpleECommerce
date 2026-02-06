using MediatR;
using SimpleECommerceBackend.Application.Results;

namespace SimpleECommerceBackend.Application.UseCases.Role.GetById;

public sealed record GetRoleByIdQuery(Guid RoleId) : IRequest<Result<GetRoleByIdResult>>;