using System.Linq.Expressions;
using Ambev.DeveloperEvaluation.Common.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class EfCoreRepositoryBase<TDbContext, TEntity>(TDbContext dbContext) : EfCoreRepositoryBase<TDbContext, TEntity, Guid>(dbContext),  IRepository<TEntity>
    where TEntity : class, IEntity<Guid>
    where TDbContext : DbContext
{
    
}

public class EfCoreRepositoryBase<TDbContext, TEntity, TKey>(TDbContext dbContext) : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TDbContext : DbContext
    where TKey : struct
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    /// <inheritdoc />
    public async Task<TEntity?> FirstOrDefaultAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TJ?> FirstOrDefaultAsync<TJ>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TJ>> selectPredicate, string[] includes,
        CancellationToken cancellationToken = default) where TJ : class
    {
        // Começa a query base no DbSet com tracking habilitado
        IQueryable<TEntity> query = _dbSet.AsQueryable().AsTracking();

        // Aplica os includes, se houver
        if (includes.Any())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        // Aplica o filtro (where)
        query = query.Where(predicate);

        // Aplica o select e retorna o primeiro ou default
        return await query
            .Select(selectPredicate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await _dbSet.Where(predicate).AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TJ>> FindAsPaginatedOrderAsync<TJ>(int currentPage, int pageSize, string orderBy, bool orderDesc,
        Expression<Func<TEntity, TJ>> selectPredicate, Expression<Func<TEntity, object>>? orderThenByPredicate = null, Expression<Func<TEntity, bool>>? query = null,
        string[]? includes = null, CancellationToken cancellationToken = default) where TJ : class
    {
        var queryable = _dbSet.AsQueryable();
        if (includes is not null)
            queryable = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));

        queryable = query is null ? queryable : queryable.Where(query);

        var orderByPredicate = !string.IsNullOrEmpty(orderBy)
            ? GetOrderByExpression(orderBy)
            : null;
        
        if (orderByPredicate is not null)
        {
            queryable = orderThenByPredicate is null
                ? (orderDesc ? queryable.OrderByDescending(orderByPredicate) : queryable.OrderBy(orderByPredicate))
                : (orderDesc
                    ? queryable.OrderByDescending(orderByPredicate).ThenByDescending(orderThenByPredicate)
                    : queryable.OrderBy(orderByPredicate).ThenBy(orderThenByPredicate));
        }

        return await queryable.Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(selectPredicate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return await (predicate is null ? _dbSet.CountAsync(cancellationToken) : _dbSet.CountAsync(predicate, cancellationToken));
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await _dbSet.AnyAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public Expression<Func<TEntity, bool>> CreateQuery(Expression<Func<TEntity, bool>> expression)
    {
        return expression;
    }
    
    private Expression<Func<TEntity, object>> GetOrderByExpression(string orderBy)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = typeof(TEntity).GetProperty(orderBy);
    
        if (property == null)
            throw new ArgumentException($"Property '{orderBy}' does not exist on type '{typeof(TEntity).Name}'.");

        var propertyAccess = Expression.Property(parameter, property.Name);
        var convertExpression = Expression.Convert(propertyAccess, typeof(object));
    
        return Expression.Lambda<Func<TEntity, object>>(convertExpression, parameter);
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TJ>> FindAsync<TJ>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TJ>> selectPredicate, CancellationToken cancellationToken = default) where TJ : class
    {
        return await _dbSet.Where(predicate).Select(selectPredicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TJ>> FindAsync<TJ>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TJ>> selectPredicate, string[] includes, CancellationToken cancellationToken = default) where TJ : class
    {
        return await includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include))
            .Where(predicate)
            .Select(selectPredicate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TJ>> AllAsync<TJ>(Expression<Func<TEntity, TJ>> selectPredicate, CancellationToken cancellationToken = default) where TJ : class
    {
        return await _dbSet.Select(selectPredicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> AllAsync(string[] includes, CancellationToken cancellationToken = default)
    {
        return await includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> AllAsync(bool asNoTracking, CancellationToken cancellationToken = default)
    {
        return asNoTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    /// <inheritdoc />
    public void Delete(TKey id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
}