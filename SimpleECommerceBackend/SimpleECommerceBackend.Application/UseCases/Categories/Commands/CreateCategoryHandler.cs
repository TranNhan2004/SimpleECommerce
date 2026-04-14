using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Commands;

[AutoConstructor]
public partial class CreateCategoryHandler : IUseCaseHandler<CreateCategoryCommand, CreateCategoryResult>
{
    private readonly ICategoryService _categoryService;
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<CreateCategoryResult> HandleAsync(
        CreateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userContext = _userContextHolder.GetUserContext();
        var category = Category.Create(
            request.Name,
            request.Description,
            userContext.Id
        );

        var createdCategory = _categoryService.CreateCategory(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _categoryService.InvalidateCacheByKeyAsync(CategoryCacheKey.GetAllCategory);

        return new CreateCategoryResult
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name,
            Description = createdCategory.Description,
            Status = CategoryStatusUtils.ToName(createdCategory.Status),
            AdminId = createdCategory.AdminId,
            CreatedAt = createdCategory.CreatedAt,
            UpdatedAt = createdCategory.UpdatedAt
        };
    }
}