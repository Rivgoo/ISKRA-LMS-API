using Iskra.Application.Filters.Abstractions;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Users.Abstractions.Models.Responses;

namespace Iskra.Modules.Users.Filters;

/// <summary>
/// Projects User entities into UserResponse DTOs.
/// </summary>
internal sealed class UserProjector : IQueryProjector<User, UserResponse>
{
    public IQueryable<UserResponse> Project(IQueryable<User> source)
    {
        return source.Select(u => new UserResponse(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.MiddleName,
            u.IsActive,
            u.IsEmailConfirmed,
            u.UserRoles.Select(ur => ur.Role!.Name).ToList(),
            u.CreatedAt,
            u.UpdatedAt
        ));
    }
}