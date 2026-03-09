using MediatR;

namespace SimpleECommerceBackend.Application.Models.Auth;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;
