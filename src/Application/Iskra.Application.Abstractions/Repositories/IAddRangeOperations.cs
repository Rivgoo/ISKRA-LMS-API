using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// Defines a method to add a collection of entities to the underlying data store in a single operation.
/// </summary>
/// <typeparam name="TEntity">The type of entity to add. Must implement the IEntity interface.</typeparam>
public interface IAddRangeOperations<in TEntity> where TEntity : IEntity
{
    void AddRange(IEnumerable<TEntity> entities);
}
