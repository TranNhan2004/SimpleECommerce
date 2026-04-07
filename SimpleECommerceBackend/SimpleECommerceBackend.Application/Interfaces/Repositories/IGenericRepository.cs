using System.Linq.Expressions;
using SimpleECommerceBackend.Domain.Entities.Abstracts;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class, IEntity
{
    Task<T?> FindByIdAsync(Guid id, bool trackChanges = false);

    Task<T?> FindFirstByConditionAsync(
        Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool trackChanges = false
    );

    Task<IReadOnlyList<T>> FindAllAsync(bool trackChanges = false);

    Task<IReadOnlyList<T>> FindAllByConditionAsync(
        Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool trackChanges = false
    );

    T Add(T entity);
    IReadOnlyList<T> AddRange(IReadOnlyList<T> entities);
    T Delete(T entity);
    IReadOnlyList<T> DeleteRange(IReadOnlyList<T> entities);
}