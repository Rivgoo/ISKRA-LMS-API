using Iskra.Core.Domain.Common;

namespace Iskra.Application.Filters.Abstractions;

/// <summary>
/// Defines the projection logic (Select clause) to transform entities into DTOs.
/// </summary>
public interface IQueryProjector<TEntity, out TResult>
    where TEntity : IEntity
{
    IQueryable<TResult> Project(IQueryable<TEntity> source);
}