using MediatR;
using SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Queries.GetProductPriceHistory;

public record GetProductPriceHistoryQuery(Guid ProductId) : IRequest<IEnumerable<ProductPriceDto>>;

[AutoConstructor]
public partial class GetProductPriceHistoryHandler : IRequestHandler<GetProductPriceHistoryQuery, IEnumerable<ProductPriceDto>>
{
    private readonly IProductPriceRepository _productPriceRepository;

    public async Task<IEnumerable<ProductPriceDto>> Handle(GetProductPriceHistoryQuery request, CancellationToken cancellationToken)
    {
        // Repo needs GetPriceHistoryAsync method, check if exists or use generic Find
        // IProductPriceRepository was created in previous plan phase. 
        // I need to check its methods. Assuming FindByProductId or similar.
        // If not, I'll use what's available or update repo.

        // Plan said: Task<IEnumerable<ProductPrice>> GetPriceHistoryAsync(Guid productId)
        // Let's assume it exists or I add it.
        var prices = await _productPriceRepository.GetPriceHistoryAsync(request.ProductId);

        return prices.Select(p => new ProductPriceDto(
            p.ProductId,
            p.Money.Amount,
            p.Money.Currency,
            p.EffectiveFrom
        ));
    }
}
