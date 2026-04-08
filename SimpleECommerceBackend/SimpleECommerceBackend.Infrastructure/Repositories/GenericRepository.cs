using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Domain.Entities.Abstracts;

namespace SimpleECommerceBackend.Infrastructure.Repositories;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    protected readonly DbContext DbContext;

    protected GenericRepository(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<T?> FindByIdAsync(Guid id, bool trackChanges = false)
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<T?> FindFirstByConditionAsync(
        Func<IQueryable<T>, IQueryable<T>> condition,
        bool trackChanges = false
    )
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        query = condition(query);
        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<IReadOnlyList<T>> FindAllAsync(bool trackChanges = false)
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> FindAllByConditionAsync(
        Func<IQueryable<T>, IQueryable<T>> condition,
        bool trackChanges = false
    )
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        query = condition(query);
        return await query.ToListAsync();
    }

    public IQueryable<T> QueryAll(bool trackChanges = false)
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return query;
    }

    public IQueryable<T> QueryAllByCondition(
        Func<IQueryable<T>, IQueryable<T>> condition,
        bool trackChanges = false
    )
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        query = condition(query);
        return query;
    }

    public virtual T Add(T entity)
    {
        DbContext.Set<T>().Add(entity);
        return entity;
    }

    public virtual T Delete(T entity)
    {
        DbContext.Set<T>().Remove(entity);
        return entity;
    }

    public virtual IReadOnlyList<T> AddRange(IReadOnlyList<T> entities)
    {
        DbContext.Set<T>().AddRange(entities);
        return entities;
    }

    public virtual IReadOnlyList<T> DeleteRange(IReadOnlyList<T> entities)
    {
        DbContext.Set<T>().RemoveRange(entities);
        return entities;
    }
}