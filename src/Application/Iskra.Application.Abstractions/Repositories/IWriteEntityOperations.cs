using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// A composite contract for Write-Only operations.
/// </summary>
public interface IWriteEntityOperations<TEntity> :
    IAddOperations<TEntity>,
    IAddRangeOperations<TEntity>,
    IUpdateOperations<TEntity>,
    IDeleteOperations<TEntity>
    where TEntity : IEntity
{
}