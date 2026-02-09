using MediatR;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;