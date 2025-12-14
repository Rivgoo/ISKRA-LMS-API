using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// A composite contract for Read-Only access to an entity.
/// Useful for services that should not modify data (e.g., Reporting).
/// </summary>
public interface IReadEntityOperations<TEntity, in TId> :
    IGetOperations<TEntity, TId>,
    IExistsByIdOperations<TId>
    where TEntity : IBaseEntity<TId>
    where TId : notnull, IComparable<TId>
{
}