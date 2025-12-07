namespace Iskra.Core.Domain.Common;

/// <summary>
/// A base class for entities that require a unique ID and automatic auditing (timestamps).
/// </summary>
/// <typeparam name="TKey">The type of the primary key.</typeparam>
public abstract class AuditableEntity<TKey> : BaseEntity<TKey>, IAuditable
    where TKey : notnull, IComparable<TKey>
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}