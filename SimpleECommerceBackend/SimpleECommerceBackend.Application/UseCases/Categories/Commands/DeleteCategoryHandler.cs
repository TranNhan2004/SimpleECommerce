using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Commands;

public class DeleteCategoryHandler : IUseCaseHandler<DeleteCategoryCommand>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(
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

    public async Task HandleAsync(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryService.GetCategoryByIdForUpdateAsync(request.Id);

        if (category.CreatedById != _currentUserContext.Id)
        {
            throw new ForbiddenException(
                CategoryErrorCodes.AdminRequired,
                "Only the admin who created the category can delete it."
            );
        }

        category.SoftDelete(_currentUserContext.Id);

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
    }
}
