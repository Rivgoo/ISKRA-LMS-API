using Iskra.Shared.CustomConsoleFormatter.Formatters;
using Microsoft.Extensions.Logging;

namespace Iskra.Shared.CustomConsoleFormatter.Extensions;

public static class LoggerBuilderExtensions
{
    public static ILoggingBuilder AddIskraConsoleFormatter(this ILoggingBuilder builder)
    {
        return builder.AddIskraConsoleFormatter(_ => { });
    }

    public static ILoggingBuilder AddIskraConsoleFormatter(
        this ILoggingBuilder builder,
        Action<IskraConsoleFormatterOptions> configure)
    {
        builder.AddConsoleFormatter<IskraConsoleFormatter, IskraConsoleFormatterOptions>(configure);
        builder.AddConsole(options =>
        {
            options.FormatterName = "IskraFormatter";
        });

        return builder;
    }
}