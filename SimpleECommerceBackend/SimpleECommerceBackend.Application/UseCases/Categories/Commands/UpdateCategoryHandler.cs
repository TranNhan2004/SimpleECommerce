using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Commands;

public class UpdateCategoryHandler : IUseCaseHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandler(
        Serilog.ILogger logger,
        IEventDispatcher eventDispatcher,
        ICurrentUserContext currentUserContext,
        ICategoryService categoryService,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
        _currentUserContext = currentUserContext;
        _categoryService = categoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCategoryResult> HandleAsync(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryService.GetCategoryByIdForUpdateAsync(request.Id);

        if (category.CreatedById != _currentUserContext.Id)
        {
            throw new ForbiddenException(
                CategoryErrorCodes.AdminRequired,
                "Only the admin who created the category can update it."
            );
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.Status = request.Status;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventDispatcher.SendAsync(new RemoveCacheEventModel
        {
            Keys = [CategoryCacheKeys.GetCategoryKey(category.Id)],
            PrefixKeys =
            [
                CategoryCacheKeys.GetAllCategoriesPrefix,
                CategoryCacheKeys.GetAllCategoriesForAdminPrefix
            ]
        }, cancellationToken);

        return UpdateCategoryResult.FromEntity(category);

    }
}
