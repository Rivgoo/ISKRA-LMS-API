using Iskra.Core.Contracts.Abstractions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Bootstrapper.PluginManagement;

/// <summary>
/// Orchestrates the loading of modules from a flat directory structure.
/// </summary>
internal class ModuleLoader
{
    public static List<IModule> LoadModules(
        string rootPath, string[] modulesToLoad, ILoggerFactory loggerFactory)
    {
        var loadedModules = new List<IModule>();
        var logger = loggerFactory.CreateLogger<ModuleLoader>();

        string absolutePath = Path.IsPathRooted(rootPath)
            ? rootPath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootPath);

        absolutePath = Path.GetFullPath(absolutePath);

        if (!Directory.Exists(absolutePath))
        {
            logger.LogWarning("Plugin directory '{PluginDirectory}' does not exist.", absolutePath);
            return loadedModules;
        }

        foreach (var moduleName in modulesToLoad)
        {
            var module = TryLoadModule(moduleName, absolutePath, logger);
            if (module != null)
            {
                loadedModules.Add(module);
                logger.LogInformation("Loaded Module: {ModuleName}", module.Name);
            }
            else
            {
                logger.LogError("Failed to load '{ModuleName}'.", moduleName);
                throw new InvalidOperationException($"Could not load module '{moduleName}'.");
            }
        }

        return loadedModules;
    }

    private static IModule? TryLoadModule(string moduleName, string rootDirectory, ILogger logger)
    {
        try
        {
            var dllPath = Path.Combine(rootDirectory, $"{moduleName}.dll");

            if (!File.Exists(dllPath))
                throw new FileNotFoundException($"Module DLL not found: {dllPath}");

            var assembly = Assembly.LoadFrom(dllPath);

            var moduleType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (moduleType == null)
            {
                logger.LogError("Assembly '{AssemblyName}' does not contain an implementation of IModule.", assembly.GetName().Name);
                return null;
            }

            return Activator.CreateInstance(moduleType) as IModule;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading module '{ModuleName}'.", moduleName);
            return null;
        }
    }
}