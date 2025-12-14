namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// Defines an operation to determine whether an entity with the specified identifier exists.
/// </summary>
/// <typeparam name="TId">The type of the identifier used to locate the entity. Must not be null.</typeparam>
public interface IExistsByIdOperations<in TId>
    where TId : notnull
{
    Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default);
}
