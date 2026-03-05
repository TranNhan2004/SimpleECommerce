using MediatR;

namespace SimpleECommerceBackend.Application.Models.Auth.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;
