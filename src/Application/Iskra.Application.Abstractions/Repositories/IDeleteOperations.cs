using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// Defines a contract for repository operations that involve deleting entities from the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be deleted. Must implement IEntity.</typeparam>
public interface IDeleteOperations<in TEntity> where TEntity : IEntity
{
    void Remove(TEntity entity);
}
