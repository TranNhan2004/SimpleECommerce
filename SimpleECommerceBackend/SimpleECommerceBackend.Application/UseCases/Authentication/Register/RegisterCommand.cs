using MediatR;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    Guid RoleId
) : IRequest<RegisterResult>;