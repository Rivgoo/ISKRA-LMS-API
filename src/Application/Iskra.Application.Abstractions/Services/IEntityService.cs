using Iskra.Application.Results;
using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Services;

/// <summary>
/// Defines a generic service contract for managing domain entities.
/// </summary>
public interface IEntityService<TEntity, TId>
    where TEntity : IBaseEntity<TId>
    where TId : notnull, IComparable<TId>
{
    Task<Result<TEntity>> GetByIdAsync(TId entityId, CancellationToken cancellationToken = default);
    Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result> CheckExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default);

    Task<Result<TEntity>> CreateAsync(TEntity newEntity, CancellationToken cancellationToken = default);
    Task<Result<TEntity>> UpdateAsync(TEntity changedEntity, CancellationToken cancellationToken = default);
    Task<Result> DeleteByIdAsync(TId entityId, CancellationToken cancellationToken = default);
}