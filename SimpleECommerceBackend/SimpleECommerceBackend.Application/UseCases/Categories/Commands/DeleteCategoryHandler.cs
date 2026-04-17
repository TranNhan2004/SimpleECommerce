using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Commands;

[AutoConstructor]
public partial class DeleteCategoryHandler : IUseCaseHandler<DeleteCategoryCommand>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly ICategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public async Task HandleAsync(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userContext = _userContextHolder.GetActiveUserContext();
        var category = await _categoryService.GetCategoryByIdForUpdateAsync(request.Id);

        if (category.AdminId != userContext.Id)
        {
            throw new ForbiddenException(
                CategoryErrorCode.AdminRequired,
                "Only the admin who created the category can delete it."
            );
        }

        _categoryService.DeleteCategory(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}