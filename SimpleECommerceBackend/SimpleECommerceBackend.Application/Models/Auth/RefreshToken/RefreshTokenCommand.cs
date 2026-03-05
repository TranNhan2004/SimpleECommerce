using MediatR;

namespace SimpleECommerceBackend.Application.Models.Auth.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<RefreshTokenResult>;
