using Microsoft.AspNetCore.Authorization;

namespace Iskra.Api.Abstractions.Authorization;

/// <summary>
/// Specifies that the class or method requires a specific permission.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HasPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission required (e.g. "users.read").</param>
    public HasPermissionAttribute(string permission)
        : base(policy: permission)
    {
    }
}