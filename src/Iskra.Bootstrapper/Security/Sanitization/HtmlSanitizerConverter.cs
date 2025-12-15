using Ganss.Xss;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iskra.Bootstrapper.Security.Sanitization;

/// <summary>
/// A JSON Converter that strips dangerous HTML tags from strings during deserialization.
/// </summary>
internal sealed class HtmlSanitizerConverter : JsonConverter<string>
{
    private static readonly HtmlSanitizer _sanitizer = new();

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawString = reader.GetString();

        if (string.IsNullOrEmpty(rawString))
            return rawString;

        // Perform XSS cleanup
        return _sanitizer.Sanitize(rawString);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}