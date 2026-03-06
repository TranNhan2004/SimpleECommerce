using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> FindAllAsync();
    Task<Category?> FindByIdAsync(Guid id);
    Category Add(Category category);
    Category Update(Category category);
    Category Delete(Category category);
}