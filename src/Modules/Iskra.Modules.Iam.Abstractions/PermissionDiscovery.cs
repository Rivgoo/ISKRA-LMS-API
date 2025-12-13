using System.Reflection;

namespace Iskra.Modules.Iam.Abstractions;

/// <summary>
/// Utility to reflectively discover all permissions defined in code.
/// Uses caching to avoid performance penalties on repeated calls.
/// </summary>
public static class PermissionDiscovery
{
    // The backing field for the cache. 
    // It guarantees that ScanForPermissions runs only once, in a thread-safe manner.
    private static readonly Lazy<HashSet<string>> _permissionsCache = new(ScanForPermissions);

    /// <summary>
    /// Returns all unique permission strings defined in IskraPermissions.
    /// This operation is O(1) after the first call.
    /// </summary>
    public static HashSet<string> GetAllPermissions() => _permissionsCache.Value;

    /// <summary>
    /// Performs the expensive reflection scan.
    /// </summary>
    private static HashSet<string> ScanForPermissions()
    {
        var permissions = new HashSet<string>();
        var rootType = typeof(IskraPermissions);

        // 1. Get all nested classes (Users, Roles, etc.)
        // We use GetNestedTypes because permissions are grouped in static classes.
        foreach (var nestedType in rootType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
        {
            // 2. Get all public constant string fields
            // IsLiteral = const, !IsInitOnly = excludes readonly
            var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

            foreach (var field in fields)
                // Get the value of the constant
                if (field.GetValue(null) is string value && !string.IsNullOrEmpty(value))
                    permissions.Add(value);
        }

        return permissions;
    }
}