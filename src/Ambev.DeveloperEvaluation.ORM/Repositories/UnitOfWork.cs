using Ambev.DeveloperEvaluation.Common.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories = new();
    
    public UnitOfWork(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity<Guid>
    {
        if (!_repositories.ContainsKey(typeof(TEntity)))
        {
            _repositories[typeof(TEntity)] = new EfCoreRepositoryBase<TDbContext, TEntity>(_dbContext);
        }
        
        return (IRepository<TEntity>)_repositories[typeof(TEntity)];
    }

    public IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class, IEntity<TKey> where TKey : struct
    {
        if (!_repositories.ContainsKey(typeof(TEntity)))
        {
            _repositories[typeof(TEntity)] = new EfCoreRepositoryBase<TDbContext, TEntity, TKey>(_dbContext);
        }
        
        return (IRepository<TEntity, TKey>)_repositories[typeof(TEntity)];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}