using MediatR;

namespace SimpleECommerceBackend.Application.UseCases.Roles.Create;

public sealed record CreateRoleCommand(string Name)
    : IRequest<CreateRoleResult>;