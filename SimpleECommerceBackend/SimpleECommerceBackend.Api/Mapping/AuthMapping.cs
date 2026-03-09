using Mapster;
using SimpleECommerceBackend.Api.DTOs.Auth;
using SimpleECommerceBackend.Application.Models.Auth.ConfirmEmail;
using SimpleECommerceBackend.Application.Models.Users.Update;
using SimpleECommerceBackend.Application.Models.Auth.RefreshToken;
using SimpleECommerceBackend.Application.Models.Auth.Register;
using SimpleECommerceBackend.Application.Models.Auth.Login;

namespace SimpleECommerceBackend.Api.Mapping;

public class AuthMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Request
        config.NewConfig<RegisterRequest, RegisterCommand>();
        config.NewConfig<ConfirmEmailRequest, ConfirmEmailCommand>();
        config.NewConfig<LoginRequest, LoginCommand>();
        config.NewConfig<RefreshTokenRequest, RefreshTokenCommand>();

        // Response
        config.NewConfig<ConfirmEmailResult, ConfirmEmailResponse>();
        config.NewConfig<RegisterResult, RegisterResponse>();
        config
            .NewConfig<LoginResult, LoginResponse>()
            .Map(dest => dest.Sex, src => src.Sex.ToString());

        config.NewConfig<RefreshTokenResult, RefreshTokenResponse>();
    }
}