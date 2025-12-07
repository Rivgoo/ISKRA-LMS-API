using System.ComponentModel.DataAnnotations;

namespace Iskra.Core.Domain.Common;

/// <summary>
/// Represents a fundamental entity within the domain that possesses a unique identifier and auditing properties.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
public interface IBaseEntity<TId> : IEntity
    where TId : notnull, IComparable<TId>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    /// <value>The unique identifier of the entity. This property typically serves as the primary key in persistence scenarios.</value>
    [Key]
    TId Id { get; set; }
}