using Mapster;
using SimpleECommerceBackend.Api.DTOs.Auth;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Domain.Utils;

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
            .Map(dest => dest.Sex, src => src.Sex.ToString())
            .Map(dest => dest.Role, src => RoleUtils.ToKeycloakRoleName(src.Role));

        config.NewConfig<RefreshTokenResult, RefreshTokenResponse>();
    }
}