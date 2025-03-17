using System.Linq.Expressions;
using Ambev.DeveloperEvaluation.Common.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IRepository
{
    
}

public interface IRepository<TEntity, TKey> : IRepository where TEntity : class, IEntity<TKey> where TKey : struct
{
    /// <summary>
    /// Gets an entity with given primary key or null if not found.
    /// </summary>
    /// <param name="id">Primary key of the entity to get.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Entity or null.</returns>
    Task<TEntity?> FirstOrDefaultAsync(TKey id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns the first record from a query.
    /// </summary>
    /// <param name="predicate">Expression to filter the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Entity or null.</returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns the first record from a query and converts it to a DTO.
    /// </summary>
    /// <typeparam name="TJ">The type of the DTO.</typeparam>
    /// <param name="predicate">Expression to filter the query.</param>
    /// <param name="selectPredicate">Expression to select the fields for the DTO.</param>
    /// <param name="includes">Related entities to include in the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The first DTO or null.</returns>
    Task<TJ?> FirstOrDefaultAsync<TJ>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TJ>> selectPredicate, string[] includes, CancellationToken cancellationToken = default) where TJ : class;

    
    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    
    /// <summary>
    /// Check if any entity satisfies the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity"></param>
    void Update(TEntity entity);
    
    /// <summary>
    /// Database search with paginated data.
    /// </summary>
    /// <typeparam name="TJ">The type of the DTO.</typeparam>
    /// <param name="currentPage">The current page number.</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="orderBy">Expression to define the ordering of the results.</param>
    /// <param name="orderDesc">Indicates whether the ordering is descending.</param>
    /// <param name="selectPredicate">Expression to select the fields for the DTO.</param>
    /// <param name="orderThenByPredicate">Expression for secondary ordering.</param>
    /// <param name="query">Expression to filter the query.</param>
    /// <param name="includes">Related entities to include in the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A paginated list of DTOs.</returns>
    Task<IEnumerable<TJ>> FindAsPaginatedOrderAsync<TJ>(
        int currentPage,
        int pageSize,
        string orderBy,
        bool orderDesc,
        Expression<Func<TEntity, TJ>> selectPredicate,
        Expression<Func<TEntity, object>>? orderThenByPredicate = null,
        Expression<Func<TEntity, bool>>? query = null,
        string[]? includes = null,
        CancellationToken cancellationToken = default) where TJ : class;
    
    /// <summary>
    /// Counts rows in a query.
    /// </summary>
    /// <param name="predicate">Expression to filter the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The count of rows matching the predicate.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);


    /// <summary>
    /// Create an Entity query
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    Expression<Func<TEntity, bool>> CreateQuery(Expression<Func<TEntity, bool>> expression);
    
      /// <summary>
    /// Returns data from a database query.
    /// </summary>
    /// <param name="predicate">Expression to filter the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of entities.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns data from a database query and converts it to DTOs.
    /// </summary>
    /// <typeparam name="TJ">The type of the DTO.</typeparam>
    /// <param name="predicate">Expression to filter the query.</param>
    /// <param name="selectPredicate">Expression to select the fields for the DTO.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of DTOs.</returns>
    Task<IEnumerable<TJ>> FindAsync<TJ>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TJ>> selectPredicate, CancellationToken cancellationToken = default) where TJ : class;

    /// <summary>
    /// Returns data from a database query and converts it to DTOs, including related entities.
    /// </summary>
    /// <typeparam name="TJ">The type of the DTO.</typeparam>
    /// <param name="predicate">Expression to filter the query.</param>
    /// <param name="selectPredicate">Expression to select the fields for the DTO.</param>
    /// <param name="includes">Related entities to include in the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of DTOs.</returns>
    Task<IEnumerable<TJ>> FindAsync<TJ>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TJ>> selectPredicate, string[] includes, CancellationToken cancellationToken = default) where TJ : class;

    /// <summary>
    /// Returns all records of an entity.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of entities.</returns>
    Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all records of an entity with selected fields.
    /// </summary>
    /// <typeparam name="TJ">The type of the DTO.</typeparam>
    /// <param name="selectPredicate">Expression to select the fields for the DTO.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of DTOs.</returns>
    Task<IEnumerable<TJ>> AllAsync<TJ>(Expression<Func<TEntity, TJ>> selectPredicate, CancellationToken cancellationToken = default) where TJ : class;

    /// <summary>
    /// Returns all records of an entity, including related entities.
    /// </summary>
    /// <param name="includes">Related entities to include in the query.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of entities.</returns>
    Task<IEnumerable<TEntity>> AllAsync(string[] includes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all records of an entity without change tracking.
    /// </summary>
    /// <param name="asNoTracking">Indicates whether to use no-tracking queries.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of entities.</returns>
    Task<IEnumerable<TEntity>> AllAsync(bool asNoTracking, CancellationToken cancellationToken = default);


    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes an entity by primary key.
    /// </summary>
    /// <param name="id">Primary key of the entity.</param>
    void Delete(TKey id);
    
}

public interface IRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : class, IEntity<Guid>
{
}