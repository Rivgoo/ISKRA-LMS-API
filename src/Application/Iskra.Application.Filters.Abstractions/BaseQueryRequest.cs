using Iskra.Application.Filters.Abstractions.Errors;
using Iskra.Application.Results;
using Iskra.Core.Domain.Common;

namespace Iskra.Application.Filters.Abstractions;

/// <summary>
/// Provides a base implementation for query requests with validation and sorting logic.
/// </summary>
/// <typeparam name="TEntity">The target domain entity.</typeparam>
public abstract class BaseQueryRequest<TEntity> : IQueryRequest where TEntity : IEntity
{
    public int PageIndex { get; set; } = 1;
    private readonly List<SortCriteria> _sorts = [];

    public IReadOnlyList<SortCriteria> GetSorts() => _sorts;

    public Result AddSort(string property, SortDirection direction)
    {
        var propertyInfo = typeof(TEntity).GetProperty(property);
        if (propertyInfo == null)
            return QueryErrors.InvalidSortField(property);

        if (_sorts.Any(x => x.Property == property))
            return QueryErrors.DuplicateSortField(property);

        _sorts.Add(new SortCriteria(property, direction));
        return Result.Ok();
    }
}