using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> FindAllAsync();
    Task<Category?> FindByIdAsync(Guid id);
    Category Add(Category category);
    Category Update(Category category);
    Category Delete(Category category);
}