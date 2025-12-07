namespace Iskra.Core.Domain.Common;

/// <summary>
/// Defines a contract for entities that track creation and modification timestamps.
/// </summary>
public interface IAuditable : IEntity
{
    /// <summary>
    /// Gets or sets the date and time (UTC) when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time (UTC) when the entity was last updated.
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }
}