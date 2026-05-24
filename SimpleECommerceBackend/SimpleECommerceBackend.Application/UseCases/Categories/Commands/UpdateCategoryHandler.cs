using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Commands;

public class UpdateCategoryHandler : IUseCaseHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly ICategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandler(
        IUserContextHolder userContextHolder,
        ICategoryService categoryService,
        IUnitOfWork unitOfWork
    )
    {
        _userContextHolder = userContextHolder;
        _categoryService = categoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCategoryResult> HandleAsync(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userContext = _userContextHolder.GetUserContext();
        var category = await _categoryService.GetCategoryByIdForUpdateAsync(request.Id);

        if (category.AdminId != userContext.Id)
        {
            throw new ForbiddenException(
                CategoryErrorCodes.AdminRequired,
                "Only the admin who created the category can update it."
            );
        }

        category.Name = request.Name;
        category.Description = request.Description;

        if (category.Status != request.Status)
        {
            switch (request.Status)
            {
                case CategoryStatus.Active:
                    category.Activate();
                    break;
                case CategoryStatus.Inactive:
                    category.Deactivate();
                    break;
                case CategoryStatus.Archived:
                    category.Archive();
                    break;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _categoryService.InvalidateCacheAsync(
            exactKeys: [CategoryCacheKeys.GetCategoryKey(category.Id)],
            prefixKeys: [CategoryCacheKeys.GetAllCategoriesPrefix, CategoryCacheKeys.GetAllCategoriesForAdminPrefix]
        );
        return UpdateCategoryResult.FromEntity(category);

    }
}
