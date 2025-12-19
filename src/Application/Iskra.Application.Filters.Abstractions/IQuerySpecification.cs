using Iskra.Core.Domain.Common;

namespace Iskra.Application.Filters.Abstractions;

/// <summary>
/// Encapsulates the filtering logic (Where clauses) for a specific entity and request.
/// </summary>
public interface IQuerySpecification<TEntity, in TRequest>
    where TEntity : IEntity
    where TRequest : IQueryRequest
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query, TRequest request);
}