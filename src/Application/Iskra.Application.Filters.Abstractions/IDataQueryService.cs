using Iskra.Application.Results;
using Iskra.Core.Domain.Common;

namespace Iskra.Application.Filters.Abstractions;

public interface IDataQueryService<TEntity, TRequest>
    where TEntity : class, IEntity
    where TRequest : IQueryRequest
{
    /// <summary>
    /// Prepares a data query service using the specified request parameters.
    /// </summary>
    /// <param name="request">The request object containing parameters used to configure the data query service. Cannot be null.</param>
    /// <returns>An instance of a data query service configured according to the provided request.</returns>
    IDataQueryService<TEntity, TRequest> Prepare(TRequest request);

    /// <summary>
    /// Sets the maximum number of items to return per page in the query results.
    /// </summary>
    /// <param name="size">The number of items to include in each page. Must be greater than zero.</param>
    /// <returns>The current query service instance with the updated page size setting.</returns>
    IDataQueryService<TEntity, TRequest> SetPageSize(int size);

    /// <summary>
    /// Overrides the default specification for the query.
    /// </summary>
    IDataQueryService<TEntity, TRequest> UseSpecification<TSpec>()
        where TSpec : IQuerySpecification<TEntity, TRequest>;

    /// <summary>
    /// Executes the query using a resolved projector.
    /// </summary>
    Task<Result<PaginatedList<TResult>>> ExecuteAsync<TResult>(CancellationToken ct = default)
        where TResult : class;

    /// <summary>
    /// Executes the query with an explicit projector type.
    /// </summary>
    Task<Result<PaginatedList<TResult>>> ExecuteAsync<TResult, TProjector>(CancellationToken ct = default)
        where TResult : class
        where TProjector : IQueryProjector<TEntity, TResult>;
}