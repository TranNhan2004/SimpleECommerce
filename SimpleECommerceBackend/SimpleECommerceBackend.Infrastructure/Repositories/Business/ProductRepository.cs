using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public async Task<Product?> FindByIdAsync(Guid id)
    {
        return await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> FindAllAsync()
    {
        return await _db.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public Product Add(Product product)
    {
        _db.Products.Add(product);
        return product;
    }

    public Product Update(Product product)
    {
        _db.Products.Update(product);
        return product;
    }

    public Product Delete(Product product)
    {
        _db.Products.Remove(product);
        return product;
    }
}