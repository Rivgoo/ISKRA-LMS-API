using Iskra.Application.Filters.Abstractions;
using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Shared.Search;
using Iskra.Modules.Users.Abstractions.Models.Requests;

namespace Iskra.Modules.Users.Filters;

/// <summary>
/// Applies filtering rules to the user query.
/// </summary>
internal sealed class UserSpecification(ISearchProvider searchProvider) : IQuerySpecification<User, UserQueryRequest>
{
    public IQueryable<User> Apply(IQueryable<User> query, UserQueryRequest request)
    {
        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        if (request.IsEmailConfirmed.HasValue)
            query = query.Where(u => u.IsEmailConfirmed == request.IsEmailConfirmed.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerms))
        {
            query = searchProvider.ApplySearch(
                query,
                request.SearchTerms.Trim(),
                request.Method,
                u => u.Email,
                u => u.FirstName,
                u => u.LastName);
        }

        return query;
    }
}