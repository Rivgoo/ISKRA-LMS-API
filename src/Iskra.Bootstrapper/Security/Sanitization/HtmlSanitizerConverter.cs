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
        string? data = reader.GetString();

        if (string.IsNullOrEmpty(data))
            return data;

        string processed = data.Trim();

        return _sanitizer.Sanitize(processed);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}