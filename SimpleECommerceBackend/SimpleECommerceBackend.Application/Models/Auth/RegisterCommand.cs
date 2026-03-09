using MediatR;

namespace SimpleECommerceBackend.Application.Models.Auth;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role
) : IRequest<RegisterResult>;
