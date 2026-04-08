using Mapster;
using SimpleECommerceBackend.Api.DTOs.UserProfiles;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Api.Mappers;

public class UserProfileMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UpdateUserProfileRequest, UpdateUserProfileCommand>();
        config.NewConfig<UpdateUserProfileResult, UpdateUserProfileResponse>();
    }
}