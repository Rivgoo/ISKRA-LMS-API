namespace Iskra.Core.Domain.Common;

/// <summary>
/// Represents the base implementation for all entities with a unique identifier.
/// </summary>
/// <typeparam name="TKey">The type of the primary key (e.g., Guid, int, long).</typeparam>
public abstract class BaseEntity<TKey> : IBaseEntity<TKey>
    where TKey : notnull, IComparable<TKey>
{
    /// <summary>
    /// Gets or sets the unique identifier for this entity.
    /// </summary>
    public TKey Id { get; set; } = default!;
}