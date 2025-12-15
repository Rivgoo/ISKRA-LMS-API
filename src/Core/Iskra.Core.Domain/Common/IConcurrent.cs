namespace Iskra.Core.Domain.Common;

/// <summary>
/// Defines a contract for entities that require Optimistic Concurrency Control.
/// </summary>
public interface IConcurrent : IEntity
{
    /// <summary>
    /// A random token that changes every time the entity is modified.
    /// Used to detect conflicting updates.
    /// </summary>
    Guid ConcurrencyToken { get; set; }
}