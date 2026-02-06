using MediatR;
using SimpleECommerceBackend.Application.Results;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResult>>;