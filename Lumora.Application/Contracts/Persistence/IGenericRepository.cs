using Lumora.Domain.Entities.Common;
using System.Linq.Expressions;

namespace Lumora.Application.Contracts.Persistence;

public interface IGenericRepository<T> where T : BaseEntity
{
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Delete(T entity);
    void Delete(IEnumerable<T> entities);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<T?> GetFirstAsync(Expression<Func<T, bool>>? predicate = null,
                           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                           List<Expression<Func<T, object>>>? includes = null,
                           bool disableTracking = true, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                        List<Expression<Func<T, object>>>? includes = null,
                                        bool disbaleTracking = true,
                                        bool includeSoftDeleted = false,
                                        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);


}
