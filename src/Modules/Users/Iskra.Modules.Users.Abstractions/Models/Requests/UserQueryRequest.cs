using Iskra.Core.Domain.Entities;
using Iskra.Application.Filters.Abstractions;
using Iskra.Infrastructure.Shared.Search;

namespace Iskra.Modules.Users.Abstractions.Models.Requests;

/// <summary>
/// Criteria for filtering and sorting users.
/// </summary>
public class UserQueryRequest : BaseQueryRequest<User>
{
    /// <summary>
    /// Text to search for in name or email fields.
    /// </summary>
    public string? SearchTerms { get; set; }

    /// <summary>
    /// The comparison logic for the search terms.
    /// </summary>
    public SearchMethod Method { get; set; } = SearchMethod.Contains;

    /// <summary>
    /// Filter by account activity status.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filter by email verification status.
    /// </summary>
    public bool? IsEmailConfirmed { get; set; }
}