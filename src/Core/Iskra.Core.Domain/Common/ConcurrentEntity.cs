using System.ComponentModel.DataAnnotations;

namespace Iskra.Core.Domain.Common;

/// <summary>
/// A base class for critical entities that require Optimistic Concurrency Control.
/// Includes ID, Auditing (Dates), and Concurrency Token.
/// </summary>
public abstract class ConcurrentEntity<TKey> : AuditableEntity<TKey>, IConcurrent
where TKey : notnull, IComparable<TKey>
{
    [ConcurrencyCheck]
    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();
}