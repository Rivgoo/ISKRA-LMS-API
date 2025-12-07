using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Iskra.Shared.CustomConsoleFormatter.Formatters;

public class IskraConsoleFormatter : ConsoleFormatter
{
    private readonly IskraConsoleFormatterOptions _options;

    private const string ColorReset = "\x1B[0m";
    private const string ColorGray = "\x1B[90m";
    private const string ColorDarkRed = "\x1B[31m";

    public IskraConsoleFormatter(IOptions<IskraConsoleFormatterOptions> options)
        : base("IskraFormatter")
    {
        _options = options.Value;
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var logLevel = logEntry.LogLevel;
        var category = logEntry.Category;
        var eventId = logEntry.EventId;
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        var exception = logEntry.Exception;

        var timestamp = _options.UseUtcTimestamp
            ? DateTime.UtcNow.ToString(_options.TimestampFormat)
            : DateTime.Now.ToString(_options.TimestampFormat);

        if (timestamp != null)
        {
            textWriter.Write(ColorGray);
            textWriter.Write(timestamp);
            textWriter.Write(ColorReset);
        }

        var (levelString, levelColorCode) = GetLogLevelStringAndColor(logLevel);
        textWriter.Write(levelColorCode);
        textWriter.Write($"[{levelString}]");
        textWriter.Write(ColorReset);

        var sourceString = GetSourceString(category);
        textWriter.Write(ColorGray);
        textWriter.Write($"[{sourceString}]");
        textWriter.Write(ColorReset);

        if (eventId.Id != 0)
        {
            textWriter.Write(ColorGray);
            textWriter.Write($"({eventId.Id})");
            textWriter.Write(ColorReset);
        }

        textWriter.Write(": ");

        var messageLines = message.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        textWriter.WriteLine(messageLines[0]);

        for (int i = 1; i < messageLines.Length; i++)
        {
            textWriter.Write("          ");
            textWriter.WriteLine(messageLines[i]);
        }

        if (exception != null)
        {
            textWriter.Write(ColorDarkRed);
            textWriter.WriteLine(exception.ToString());
            textWriter.Write(ColorReset);
        }
    }

    private string GetSourceString(string category)
    {
        if (category.StartsWith("Microsoft."))
        {
            var parts = category.Split('.');
            return parts.Length > 2 ? $"{parts[parts.Length - 2]}.{parts[parts.Length - 1]}" : category;
        }

        var lastDot = category.LastIndexOf('.');
        return lastDot > 0 ? category.Substring(lastDot + 1) : category;
    }

    private (string, string) GetLogLevelStringAndColor(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => ("TRC", "\x1B[90m"),      // Gray
            LogLevel.Debug => ("DBG", "\x1B[36m"),      // Cyan
            LogLevel.Information => ("INF", "\x1B[32m"),// Green
            LogLevel.Warning => ("WRN", "\x1B[33m"),    // Yellow
            LogLevel.Error => ("ERR", "\x1B[31m"),      // Red
            LogLevel.Critical => ("CRT", "\x1B[1;31m"), // Bold Red
            _ => ("???", "\x1B[37m")                    // White
        };
    }
}