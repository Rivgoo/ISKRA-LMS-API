using Iskra.Application.Filters.Abstractions;
using Iskra.Application.Filters.Abstractions.Options;
using Iskra.Application.Results;
using Iskra.Core.Domain.Common;
using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Filters.Services;

internal class DataQueryService<TEntity, TRequest>(
    IServiceProvider provider,
    AppDbContextBase dbContext,
    IOptions<FilterSettingsOptions> filterOptions) : IDataQueryService<TEntity, TRequest>
    where TEntity : class, IEntity
    where TRequest : IQueryRequest
{
    private TRequest _request = default!;
    private int _pageSize = filterOptions.Value.DefaultPageSize;
    private Type? _customSpecificationType;

    public IDataQueryService<TEntity, TRequest> Prepare(TRequest request)
    {
        _request = request;
        return this;
    }

    public IDataQueryService<TEntity, TRequest> SetPageSize(int size)
    {
        var settings = filterOptions.Value;
        _pageSize = size <= 0 ? settings.DefaultPageSize : Math.Min(size, settings.MaxPageSize);
        return this;
    }

    public IDataQueryService<TEntity, TRequest> UseSpecification<TSpec>()
        where TSpec : IQuerySpecification<TEntity, TRequest>
    {
        _customSpecificationType = typeof(TSpec);
        return this;
    }

    public async Task<Result<PaginatedList<TResult>>> ExecuteAsync<TResult>(CancellationToken ct = default)
        where TResult : class
    {
        return await ExecuteInternalAsync<TResult>(null, ct);
    }

    public async Task<Result<PaginatedList<TResult>>> ExecuteAsync<TResult, TProjector>(CancellationToken ct = default)
        where TResult : class
        where TProjector : IQueryProjector<TEntity, TResult>
    {
        return await ExecuteInternalAsync<TResult>(typeof(TProjector), ct);
    }

    private async Task<Result<PaginatedList<TResult>>> ExecuteInternalAsync<TResult>(Type? projectorType, CancellationToken ct)
        where TResult : class
    {
        var source = dbContext.Set<TEntity>().AsNoTracking();

        // 1. Resolve and Apply Specification
        var specType = _customSpecificationType ?? typeof(IQuerySpecification<TEntity, TRequest>);
        var spec = (IQuerySpecification<TEntity, TRequest>)provider.GetRequiredService(specType);
        var filteredQuery = spec.Apply(source, _request);

        // 2. Statistics
        var total = await filteredQuery.CountAsync(ct);

        // 3. Sorting
        var sortedQuery = ApplySorting(filteredQuery, _request.GetSorts());

        // 4. Resolve and Apply Projector
        var projType = projectorType ?? typeof(IQueryProjector<TEntity, TResult>);
        var projector = (IQueryProjector<TEntity, TResult>)provider.GetRequiredService(projType);
        var projectedQuery = projector.Project(sortedQuery);

        // 5. Materialization
        int safePageIndex = _request.PageIndex < 1 ? 1 : _request.PageIndex;
        int skipCount = (safePageIndex - 1) * _pageSize;

        var items = await projectedQuery
            .Skip(skipCount)
            .Take(_pageSize)
            .ToListAsync(ct);

        var list = new PaginatedList<TResult>(items, safePageIndex, _pageSize).SetTotalCount(total);
        return Result<PaginatedList<TResult>>.Ok(list);
    }

    private static IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, IReadOnlyList<SortCriteria> sorts)
    {
        if (sorts.Count == 0) return query;

        return sorts.Aggregate(query, (current, sort) => current.ApplySort(sort.Property, sort.Direction));
    }
}