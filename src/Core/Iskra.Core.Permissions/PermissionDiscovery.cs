using System.Reflection;

namespace Iskra.Core.Permissions;

/// <summary>
/// Utility to reflectively discover all permissions defined in code.
/// Useful for populating UI selection lists.
/// </summary>
public static class PermissionDiscovery
{
    public static IEnumerable<string> GetAllPermissions()
    {
        var rootType = typeof(IskraPermissions);

        foreach (var nestedType in rootType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
        {
            var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = field.GetValue(null) as string;
                if (!string.IsNullOrEmpty(value))
                    yield return value;
            }
        }
    }
}