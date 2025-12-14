using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// Defines a contract for repository operations that involve adding entities to the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be added. Must implement IEntity.</typeparam>
public interface IAddOperations<in TEntity> where TEntity : IEntity
{
    void Add(TEntity entity);
}
