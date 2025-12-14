using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// Defines a contract for repository operations that involve updating entities in the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be updated. Must implement IEntity.</typeparam>
public interface IUpdateOperations<in TEntity> where TEntity : IEntity
{
    void Update(TEntity entity);
}
