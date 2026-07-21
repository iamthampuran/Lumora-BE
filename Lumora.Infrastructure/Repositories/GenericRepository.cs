using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Common;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lumora.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _appDbContext;

    public GenericRepository(AppDbContext appDbContext) => _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));

    public Task Add(T entity)
    {
        var addedEntity = _appDbContext.Add(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _appDbContext.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _appDbContext.Set<T>().CountAsync(predicate, cancellationToken);
    }

    public void Delete(T entity)
    {
        _appDbContext.Set<T>().Remove(entity);
    }

    public void Delete(IEnumerable<T> entities)
    {
        _appDbContext.Set<T>().RemoveRange(entities);
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _appDbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken);

    }

    public async Task<IReadOnlyList<T>> GetAsyncc(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
        List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true, bool includeSoftDeleted = false, CancellationToken cancellationToken)
    {
        IQueryable<T> query =  _appDbContext.Set<T>();
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (includeSoftDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        if (orderBy != null)
        {
            return  await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _appDbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _appDbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> GetFirstAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, 
        IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true, CancellationToken cancellationToken)
    {
        IQueryable<T> query = _appDbContext.Set<T>();
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            return await orderBy(query).FirstOrDefaultAsync();
        }

        return await query.FirstOrDefaultAsync();
    }
}
