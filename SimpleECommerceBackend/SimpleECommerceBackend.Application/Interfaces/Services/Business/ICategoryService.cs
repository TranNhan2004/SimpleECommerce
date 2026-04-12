using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface ICategoryService
{
    Task<IReadOnlyCollection<Category>> GetAllCategoriesAsync();
}