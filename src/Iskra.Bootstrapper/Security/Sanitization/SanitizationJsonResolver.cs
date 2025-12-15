using Iskra.Bootstrapper.Options;
using Iskra.Core.Contracts.Attributes;
using System.Text.Json.Serialization.Metadata;

namespace Iskra.Bootstrapper.Security.Sanitization;

internal static class SanitizationJsonResolver
{
    public static Action<JsonTypeInfo> CreateModifier(SanitizationOptions options)
    {
        var converter = new HtmlSanitizerConverter();
        var excludedNames = options.ExcludedProperties.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return (typeInfo) =>
        {
            if (typeInfo.Kind != JsonTypeInfoKind.Object) return;

            foreach (var prop in typeInfo.Properties)
            {
                // 1. Must be a string
                if (prop.PropertyType != typeof(string)) continue;

                // 2. Check explicit Attribute [DisableSanitization]
                if (prop.AttributeProvider != null &&
                    prop.AttributeProvider.IsDefined(typeof(DisableSanitizationAttribute), true))
                    continue;

                // 3. Check Global Exclusion List (e.g. "Password")
                if (excludedNames.Contains(prop.Name))
                    continue;

                // 4. Apply Sanitizer
                prop.CustomConverter = converter;
            }
        };
    }
}
