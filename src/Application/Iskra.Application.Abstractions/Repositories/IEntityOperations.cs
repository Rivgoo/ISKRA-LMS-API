using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// A comprehensive contract for standard CRUD operations on an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public interface IEntityOperations<TEntity, in TId> :
    IAddOperations<TEntity>,
    IAddRangeOperations<TEntity>,
    IGetOperations<TEntity, TId>,
    IExistsByIdOperations<TId>,
    IUpdateOperations<TEntity>,
    IDeleteOperations<TEntity>
    where TEntity : IBaseEntity<TId>
    where TId : notnull, IComparable<TId>
{
}