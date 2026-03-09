using MediatR;

namespace SimpleECommerceBackend.Application.Models.Auth;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<RefreshTokenResult>;
