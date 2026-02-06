using MediatR;
using SimpleECommerceBackend.Application.Results;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    Guid RoleId
) : IRequest<Result<RegisterResult>>;