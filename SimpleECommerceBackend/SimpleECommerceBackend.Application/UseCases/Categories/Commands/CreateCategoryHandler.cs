using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Commands;

public class CreateCategoryHandler : IUseCaseHandler<CreateCategoryCommand, CreateCategoryResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly ICategoryService _categoryService;
    private readonly ICurrentUserContextProvider _userContextHolder;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(
        Serilog.ILogger logger,
        ICategoryService categoryService,
        ICurrentUserContextProvider userContextHolder,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _categoryService = categoryService;
        _userContextHolder = userContextHolder;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCategoryResult> HandleAsync(
        CreateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userContext = _userContextHolder.GetUserContext();
        _logger.Information("User {UserId} is creating a new category with name: {CategoryName}", userContext.Id, request.Name);

        var category = new Category
        {
            Id = UuidUtils.CreateV7(),
            Name = request.Name,
            Description = request.Description,
            Status = CategoryStatus.Active,
            AdminId = userContext.Id
        };

        _logger.Information("Creating category: {CategoryName}", category.Name);
        var createdCategory = _categoryService.CreateCategory(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Information("Category created with ID: {CategoryId}", createdCategory.Id);

        await _categoryService.InvalidateCacheAsync(
            prefixKeys: [CategoryCacheKeys.GetAllCategoriesPrefix, CategoryCacheKeys.GetAllCategoriesForAdminPrefix]
        );

        _logger.Information("Cache invalidated for keys with prefixes: {Prefixes}", [CategoryCacheKeys.GetAllCategoriesPrefix, CategoryCacheKeys.GetAllCategoriesForAdminPrefix]);
        _logger.Information("CreateCategoryHandler completed successfully.");
        return CreateCategoryResult.FromEntity(createdCategory);
    }
}
