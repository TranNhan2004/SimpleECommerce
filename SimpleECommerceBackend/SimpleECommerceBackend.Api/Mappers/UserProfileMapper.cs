using Mapster;
using SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Api.Mappers;

public class UserProfileMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GetMyProfileRequest, GetMyProfileQuery>();
        config.NewConfig<GetMyProfileResult, GetMyProfileResponse>();

        config.NewConfig<UpdateMyProfileRequest, UpdateMyProfileCommand>();
        config.NewConfig<UpdateMyProfileResult, UpdateMyProfileResponse>();

        config.NewConfig<CreateMyProfileRequest, CreateMyProfileCommand>();
        config.NewConfig<CreateMyProfileResult, CreateMyProfileResponse>();

        config.NewConfig<DeleteMyProfileRequest, DeleteMyProfileCommand>();
    }
}