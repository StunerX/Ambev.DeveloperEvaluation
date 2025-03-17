using Ambev.DeveloperEvaluation.Common.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IUnitOfWork
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity<Guid>;
    
    IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class, IEntity<TKey> where TKey : struct;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}