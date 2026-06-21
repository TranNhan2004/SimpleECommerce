using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Commands;

public class CreateCategoryHandler : IUseCaseHandler<CreateCategoryCommand, CreateCategoryResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(
        Serilog.ILogger logger,
        IEventDispatcher eventDispatcher,
        ICategoryService categoryService,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
        _categoryService = categoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCategoryResult> HandleAsync(
        CreateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = new Category
        {
            Id = UuidUtils.CreateV7(),
            Name = request.Name,
            Description = request.Description,
            Status = CategoryStatus.Active
        };

        var createdCategory = _categoryService.CreateCategory(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventDispatcher.SendAsync(new RemoveCacheEventModel
        {
            PrefixKeys =
            [
                CategoryCacheKeys.GetAllCategoriesPrefix,
                CategoryCacheKeys.GetAllCategoriesForAdminPrefix
            ]
        }, cancellationToken);

        return CreateCategoryResult.FromEntity(createdCategory);
    }
}
