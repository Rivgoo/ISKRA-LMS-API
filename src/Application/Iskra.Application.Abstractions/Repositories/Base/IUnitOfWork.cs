namespace Iskra.Application.Abstractions.Repositories.Base;

/// <summary>
/// Represents the Unit of Work pattern to group repository operations into a single transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves all changes made to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}