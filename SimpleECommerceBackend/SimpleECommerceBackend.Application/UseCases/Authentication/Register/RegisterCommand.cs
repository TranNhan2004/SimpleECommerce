using MediatR;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    Role Role
) : IRequest<RegisterResult>;