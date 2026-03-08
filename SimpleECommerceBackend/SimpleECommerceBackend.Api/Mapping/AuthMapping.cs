using Mapster;

using SimpleECommerceBackend.Api.DTOs.Auth;
using SimpleECommerceBackend.Application.Models.Auth.Login;
using SimpleECommerceBackend.Application.Models.Auth.RefreshToken;
using SimpleECommerceBackend.Application.Models.Auth.Register;

namespace SimpleECommerceBackend.Api.Mapping;

public class AuthMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Request
        config.NewConfig<RegisterRequest, RegisterCommand>();
        config.NewConfig<LoginRequest, LoginCommand>();
        config.NewConfig<RefreshTokenRequest, RefreshTokenCommand>();

        // Response
        config.NewConfig<RegisterResult, RegisterResponse>();
        config.NewConfig<LoginResult, LoginResponse>();
        config.NewConfig<RefreshTokenResult, RefreshTokenResponse>();
    }
}