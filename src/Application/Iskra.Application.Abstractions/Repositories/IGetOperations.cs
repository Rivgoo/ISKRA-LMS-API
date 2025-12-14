using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

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