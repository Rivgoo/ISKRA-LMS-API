using Iskra.Application.Filters.Abstractions;
using Iskra.Application.Results;

namespace Iskra.Api.Abstractions.Filters;

/// <summary>
/// Provides logic to apply public sorting parameters to query requests.
/// </summary>
public static class QueryRequestExtensions
{
    /// <summary>
    /// Loops through sorting parameters and registers them in the request.
    /// </summary>
    /// <param name="request">The target query request.</param>
    /// <param name="paramsObj">The raw sorting parameters from the API.</param>
    /// <returns>A result indicating success or the first encountered error.</returns>
    public static Result ApplySorts(this IQueryRequest request, SortParams? paramsObj)
    {
        if (paramsObj?.Fields == null || paramsObj.Directions == null)
            return Result.Ok();

        for (int i = 0; i < paramsObj.Fields.Length; i++)
        {
            if (paramsObj.Directions.Length <= i)
                break;

            var result = request.AddSort(paramsObj.Fields[i], paramsObj.Directions[i]);

            if (result.IsFailure)
                return result;
        }

        return Result.Ok();
    }
}