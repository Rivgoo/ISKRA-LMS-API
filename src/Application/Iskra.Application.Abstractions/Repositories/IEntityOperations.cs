using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// A comprehensive contract for full CRUD operations.
/// Combines Read and Write capabilities.
/// </summary>
public interface IEntityOperations<TEntity, in TId> :
    IReadEntityOperations<TEntity, TId>,
    IWriteEntityOperations<TEntity>
    where TEntity : IBaseEntity<TId>
    where TId : notnull, IComparable<TId>
{
}