using Mapster;
using SimpleECommerceBackend.Api.DTOs.Auth;
using SimpleECommerceBackend.Application.UseCases.Auth.Login;
using SimpleECommerceBackend.Application.UseCases.Auth.Register;

namespace SimpleECommerceBackend.Api.Mapping;

public class AuthMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterResult, RegisterResponse>();
        config.NewConfig<LoginResult, LoginResponse>();
    }
}