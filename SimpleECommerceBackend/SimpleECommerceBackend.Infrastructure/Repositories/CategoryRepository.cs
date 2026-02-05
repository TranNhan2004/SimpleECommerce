using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories;

namespace SimpleECommerceBackend.Infrastructure.Repositories;

[AutoConstructor]
internal partial class CategoryRepository : ICategoryRepository
{
    private readonly DbContext _dbContext;

    private DbSet<Category> Categories => _dbContext.Set<Category>();

    public async Task<IEnumerable<Category>> FindAllAsync()
    {
        return await Categories
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<Category?> FindByIdAsync(Guid id)
    {
        return await Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<Category> AddAsync(Category category)
    {
        await Categories.AddAsync(category);
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        Categories.Update(category);
        return category;
    }

    public async Task<Category> DeleteAsync(Category category)
    {
        // Soft delete – entity tự chịu trách nhiệm invariant
        Categories.Update(category);
        return category;
    }
}