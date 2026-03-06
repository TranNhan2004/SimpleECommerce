using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories;

[AutoConstructor]
public partial class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public async Task<IReadOnlyList<Category>> FindAllAsync()
    {
        return await _db.Categories.ToListAsync();
    }

    public async Task<Category?> FindByIdAsync(Guid id)
    {
        return await _db.Categories.FindAsync(id);
    }

    public Category Add(Category category)
    {
        _db.Categories.Add(category);
        return category;
    }

    public Category Update(Category category)
    {
        _db.Categories.Update(category);
        return category;
    }

    public Category Delete(Category category)
    {
        _db.Categories.Remove(category);
        return category;
    }
}