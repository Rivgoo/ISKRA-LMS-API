using Iskra.Api.Host.PluginManagement;
using Iskra.Shared.CustomConsoleFormatter.Extensions;

namespace Iskra.Api.Host;

/// <summary>
/// The main entry point for the Iskra Modular Platform.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Logging Configuration
        var loggingSection = builder.Configuration.GetSection("Logging:CustomFormatter");
        builder.Logging.ClearProviders();
        builder.Logging.AddIskraConsoleFormatter(options =>
        {
            options.TimestampFormat = loggingSection.GetValue<string>("TimestampFormat");
            options.UseUtcTimestamp = loggingSection.GetValue<bool>("UseUtcTimestamp");
        });

        // Temporarily create a logger factory for startup logging
        using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddIskraConsoleFormatter(options =>
            {
                options.TimestampFormat = loggingSection.GetValue<string>("TimestampFormat");
                options.UseUtcTimestamp = loggingSection.GetValue<bool>("UseUtcTimestamp");
            });
        });

        var programLogger = loggerFactory.CreateLogger<Program>();

        // Host Configuration
        var pluginSettings = builder.Configuration.GetSection("PluginSettings");

        var rootPath = pluginSettings["RootPath"];

        if (string.IsNullOrWhiteSpace(rootPath))
            throw new InvalidOperationException("Fatal: 'PluginSettings:RootPath' is missing.");

        var enabledModules = pluginSettings.GetSection("EnabledModules").Get<string[]>();

        if (enabledModules == null || enabledModules.Length == 0)
        {
            programLogger.LogWarning("No modules are listed in 'PluginSettings:EnabledModules'. System will start empty.");
            enabledModules = [];
        }

        var enableCleaner = pluginSettings.GetValue<bool>("EnableCleaner");

        var absoluteRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootPath));

        // Cleanup Modules
        if (enableCleaner)
            ModuleCleaner.Clean(absoluteRootPath, enabledModules, loggerFactory.CreateLogger<ModuleCleaner>());
        else
            programLogger.LogInformation("Module cleanup is disabled in configuration.");

        // Initialize Assembly Resolver
        AssemblyResolver.Initialize(absoluteRootPath, loggerFactory.CreateLogger<AssemblyResolver>());

        // Basic API Setup
        var mvcBuilder = builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Load Modules
        var environment = builder.Environment.EnvironmentName;
        var loadedModules = ModuleLoader.LoadModules(absoluteRootPath, enabledModules, environment, loggerFactory);

        // Register Services & Controllers
        foreach (var (module, config) in loadedModules)
        {
            try
            {
                module.RegisterServices(builder.Services, config, loggerFactory);
                mvcBuilder.AddApplicationPart(module.Assembly);
            }
            catch (Exception ex)
            {
                programLogger.LogError("Failed to register services for module '{ModuleName}': {ErrorMessage}", module.Name, ex.Message);
                throw;
            }
        }

        var app = builder.Build();

        // Configure Middleware Pipeline (Ordered by Priority)
        foreach (var (module, _) in loadedModules.OrderBy(x => x.Module.Priority))
            module.ConfigureMiddleware(app);

        app.MapControllers();

        // Final
        PrintBanner(loadedModules.Count, environment);

        app.Run();
    }

    private static void PrintBanner(int loadedModuleCount, string environment)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(@"
       ___ ____  _  ______      _      
      |_ _/ ___|| |/ /  _ \    / \   
       | |\___ \| ' /| |_) |  / _ \  
       | | ___) | . \|  _ <  / ___ \ 
      |___|____/|_|\_\_| \_\/_/   \_\

        Modular Learning Platform
             by Pavlo Panko
");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine($"[Host] Total Modules Active: {loadedModuleCount}");
        Console.WriteLine($"[Host] Environment: {environment}");
        Console.WriteLine("---------------------------------------------------------");
        Console.ResetColor();
    }
}