using Iskra.Bootstrapper.Middleware;
using Iskra.Bootstrapper.Options;
using Iskra.Bootstrapper.PluginManagement;
using Iskra.Bootstrapper.Security;
using Iskra.Bootstrapper.Security.Sanitization;
using Iskra.Core.Contracts.Constants;
using Iskra.Shared.CustomConsoleFormatter.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Iskra.Bootstrapper.Extensions;

public static class IskraBuilderExtensions
{
    /// <summary>
    /// Bootstraps the Iskra Platform, loads modules, and registers services.
    /// </summary>
    public static IskraEngine AddIskraPlatform(this WebApplicationBuilder builder)
    {
        // Setup Logging
        ConfigureLogging(builder);

        //  Create Temporary Logger for Startup
        using var loggerFactory = LoggerFactory.Create(lb =>
        {
            lb.AddConfiguration(builder.Configuration.GetSection("Logging"));
            lb.AddConsole();
            lb.AddIskraConsoleFormatter();
        });
        var logger = loggerFactory.CreateLogger("Iskra.Bootstrapper");

        //  Bind Options
        var securitySection = builder.Configuration.GetSection(SecurityOptions.SectionName);
        var securityOptions = securitySection.Get<SecurityOptions>() ?? throw new InvalidOperationException("Failed to bind SecurityOptions from configuration.");

        builder.Services.Configure<SecurityOptions>(securitySection);

        // Register Global Exception Handler
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Configure Host Filtering
        var hostConfig = securityOptions.HostFiltering;

        builder.Services.Configure<HostFilteringOptions>(options =>
        {
            options.AllowedHosts = hostConfig.AllowedHosts.Split(';', StringSplitOptions.RemoveEmptyEntries);
            options.AllowEmptyHosts = hostConfig.AllowEmptyHosts;
            options.IncludeFailureMessage = hostConfig.IncludeFailureMessage;
        });

        // Security Checks
        SecurityChecks.PerformChecks(securityOptions, logger);

        // Configure CORS
        CorsConfigurator.ConfigureCors(builder.Services, securityOptions);

        // Configure Rate Limiting
        RateLimitingExtensions.AddIskraRateLimiting(builder.Services, securityOptions);

        // Configure API Versioning
        ApiVersioningExtensions.AddIskraApiVersioning(builder.Services);

        // Initialize Assembly Resolver
        AssemblyResolver.Initialize(loggerFactory.CreateLogger<AssemblyResolver>());

        // Read Configuration
        var pluginSettings = builder.Configuration.GetSection("PluginSettings");
        var enabledModules = pluginSettings.GetSection("EnabledModules").Get<string[]>() ?? [];
        var systemModules = builder.Configuration.GetSection("SystemModules").Get<string[]>() ?? [];

        var allModules = enabledModules.Concat(systemModules).Distinct().ToArray();

        // Load Modules
        var environment = builder.Environment.EnvironmentName;
        var loadedModules = ModuleLoader.LoadModules(allModules, loggerFactory);

        // Configure JSON Sanitization
        var sanitizationModifier = SanitizationJsonResolver.CreateModifier(securityOptions.Sanitization);

        // Register Services
        var mvcBuilder = builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                if (securityOptions.Sanitization.Enabled)
                    options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
                    {
                        Modifiers = { sanitizationModifier }
                    };
            });

        foreach (var module in loadedModules)
        {
            module.RegisterServices(builder.Services, builder.Configuration, loggerFactory);
            mvcBuilder.AddApplicationPart(module.Assembly);
        }

        return new IskraEngine(loadedModules, environment, securityOptions);
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        var loggingSection = builder.Configuration.GetSection("Logging:CustomFormater");

        builder.Logging.ClearProviders();
        builder.Logging.AddIskraConsoleFormatter(options =>
        {
            options.TimestampFormat = loggingSection.GetValue<string>("TimestampFormat");
            options.UseUtcTimestamp = loggingSection.GetValue<bool>("UseUtcTimestamp");
        });
    }
}