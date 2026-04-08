using System.Linq.Expressions;
using SimpleECommerceBackend.Domain.Entities.Abstracts;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class, IEntity
{
    Task<T?> FindByIdAsync(Guid id, bool trackChanges = false);

    Task<T?> FindFirstByConditionAsync(
        Func<IQueryable<T>, IQueryable<T>> condition,
        bool trackChanges = false
    );

    IQueryable<T> QueryAll(bool trackChanges = false);

    IQueryable<T> QueryAllByCondition(
        Func<IQueryable<T>, IQueryable<T>> condition,
        bool trackChanges = false
    );

    Task<IReadOnlyList<T>> FindAllAsync(bool trackChanges = false);

    Task<IReadOnlyList<T>> FindAllByConditionAsync(
        Func<IQueryable<T>, IQueryable<T>> condition,
        bool trackChanges = false
    );

    T Add(T entity);
    IReadOnlyList<T> AddRange(IReadOnlyList<T> entities);
    T Delete(T entity);
    IReadOnlyList<T> DeleteRange(IReadOnlyList<T> entities);
}