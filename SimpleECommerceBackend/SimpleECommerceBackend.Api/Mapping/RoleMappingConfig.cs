using Mapster;
using SimpleECommerceBackend.Api.DTOs.Roles;
using SimpleECommerceBackend.Application.UseCases.Roles.Create;
using SimpleECommerceBackend.Application.UseCases.Roles.GetAll;
using SimpleECommerceBackend.Application.UseCases.Roles.GetById;
using SimpleECommerceBackend.Application.UseCases.Roles.Update;

namespace SimpleECommerceBackend.Api.Mapping;

public sealed class RoleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Request DTO -> Command
        config.NewConfig<CreateRoleRequest, CreateRoleCommand>();

        config.NewConfig<(Guid roleId, UpdateRoleRequest dto), UpdateRoleCommand>()
            .Map(dest => dest.RoleId, src => src.roleId)
            .Map(dest => dest.Name, src => src.dto.Name);

        // Result -> Response DTO
        config.NewConfig<GetAllRolesResult.RoleItem, RoleResponse>();
        config.NewConfig<GetRoleByIdResult, RoleResponse>();
    }
}