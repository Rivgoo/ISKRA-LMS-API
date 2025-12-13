using Iskra.Bootstrapper.Options;
using Iskra.Bootstrapper.Security;
using Iskra.Core.Contracts.Abstractions;
using Microsoft.AspNetCore.Builder;

namespace Iskra.Bootstrapper;

/// <summary>
/// Represents the running state of the Iskra Modular Platform.
/// Holds references to loaded modules to configure the pipeline later.
/// </summary>
public class IskraEngine
{
    private readonly List<IModule> _loadedModules;
    private readonly string _environment;
    private readonly SecurityOptions _securityOptions;

    internal IskraEngine(
        List<IModule> modules,
        string environment,
        SecurityOptions securityOptions)
    {
        _loadedModules = modules;
        _environment = environment;
        _securityOptions = securityOptions;
    }


    /// <summary>
    /// Configures the HTTP middleware pipeline for all loaded modules.
    /// </summary>
    public void UseIskra(WebApplication app)
    {
        // 1. Apply AllowedHosts
        app.UseHostFiltering();

        // Apply CORS
        CorsConfigurator.UseConfiguredCors(app, _securityOptions);

        // Sort by Priority
        var sortedModules = _loadedModules.OrderBy(x => x.Priority).ToList();

        foreach (var module in sortedModules)
            module.ConfigureMiddleware(app);

        PrintBanner(sortedModules.Count, _environment);
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