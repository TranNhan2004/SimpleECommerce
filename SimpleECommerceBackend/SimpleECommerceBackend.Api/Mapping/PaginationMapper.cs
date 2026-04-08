using Mapster;
using SimpleECommerceBackend.Api.DTOs.Pagination;
using SimpleECommerceBackend.Application.Models.Pagination;

namespace SimpleECommerceBackend.Api.Mapping;

public class PaginationMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PaginationRequest, PaginationRequestModel>();
        config.NewConfig(typeof(PaginationResultModel<>), typeof(PaginationResponse<>));
    }
}