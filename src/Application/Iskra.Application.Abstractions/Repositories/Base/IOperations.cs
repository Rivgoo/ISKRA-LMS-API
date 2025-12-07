using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories.Base;

/// <summary>
/// Defines a contract for repository operations that involve adding entities to the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be added. Must implement IEntity.</typeparam>
public interface IAddOperations<in TEntity> where TEntity : IEntity
{
    void Add(TEntity entity);
}

/// <summary>
/// Defines a method to add a collection of entities to the underlying data store in a single operation.
/// </summary>
/// <typeparam name="TEntity">The type of entity to add. Must implement the IEntity interface.</typeparam>
public interface IAddRangeOperations<in TEntity> where TEntity : IEntity
{
    void AddRange(IEnumerable<TEntity> entities);
}

/// <summary>
/// Defines a contract for repository operations that involve updating entities in the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be updated. Must implement IEntity.</typeparam>
public interface IUpdateOperations<in TEntity> where TEntity : IEntity
{
    void Update(TEntity entity);
}

/// <summary>
/// Defines a contract for repository operations that involve deleting entities from the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be deleted. Must implement IEntity.</typeparam>
public interface IDeleteOperations<in TEntity> where TEntity : IEntity
{
    void Remove(TEntity entity);
}

/// <summary>
/// Defines an operation to determine whether an entity with the specified identifier exists.
/// </summary>
/// <typeparam name="TId">The type of the identifier used to locate the entity. Must not be null.</typeparam>
public interface IExistsByIdOperations<in TId>
    where TId : notnull
{
    Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines operations for retrieving entities by identifier or retrieving all entities of a specified type.
/// </summary>
/// <typeparam name="TEntity">The type of entity to be retrieved. Must implement <see cref="IEntity"/>.</typeparam>
/// <typeparam name="TId">The type of the unique identifier for the entity. Must be non-nullable.</typeparam>
public interface IGetOperations<TEntity, in TId>
    where TEntity : IEntity
    where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}