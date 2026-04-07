using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    public UnitOfWork(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}