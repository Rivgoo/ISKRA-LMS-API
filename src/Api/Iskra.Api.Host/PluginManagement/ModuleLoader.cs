using Iskra.Core.Contracts.Abstractions;
using System.Reflection;

namespace Iskra.Api.Host.PluginManagement;

/// <summary>
/// Orchestrates the loading of modules from a flat directory structure with detailed error reporting.
/// </summary>
internal class ModuleLoader
{
    /// <summary>
    /// Scans a flat directory for assemblies implementing IModule and loads their configurations.
    /// </summary>
    /// <param name="rootPath">The directory containing all module DLLs and configs.</param>
    /// <param name="environmentName">The current hosting environment.</param>
    /// <returns>A list of loaded modules and their specific configurations.</returns>
    public static List<(IModule Module, IConfiguration Config)> LoadModules(
    string rootPath, string[] modulesToLoad, string environmentName, ILoggerFactory loggerFactory)
    {
        var loadedModules = new List<(IModule, IConfiguration)>();
        var logger = loggerFactory.CreateLogger<ModuleLoader>();

        string absolutePath;
        if (Path.IsPathRooted(rootPath))
            absolutePath = rootPath;
        else
            absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootPath);

        absolutePath = Path.GetFullPath(absolutePath);

        if (!Directory.Exists(absolutePath))
        {
            logger.LogWarning("Plugin directory '{PluginDirectory}' did not exist. Creating it now.", absolutePath);
            Directory.CreateDirectory(absolutePath);

            return loadedModules;
        }

        logger.LogInformation("Processing {ModuleCount} modules from: {PluginDirectory}", modulesToLoad.Length, absolutePath);

        foreach (var moduleName in modulesToLoad)
        {
            var (Module, Config) = TryLoadModule(moduleName, absolutePath, environmentName, logger, loggerFactory);

            if (Module != null)
            {
                loadedModules.Add((Module, Config!));
                logger.LogInformation("Loaded: {ModuleName}", Module.Name);
            }
            else
            {
                logger.LogError("Failed to load '{ModuleName}'", moduleName);
                throw new InvalidOperationException($"File found for '{moduleName}', but it could not be loaded as an IModule.");
            }
        }

        return loadedModules;
    }

    private static (IModule? Module, IConfiguration? Config) TryLoadModule(
        string moduleName, string rootDirectory, string envName, ILogger<ModuleLoader> logger, ILoggerFactory loggerFactory)
    {
        try
        {
            var dllPath = Path.Combine(rootDirectory, $"{moduleName}.dll");

            if (!File.Exists(dllPath))
                throw new FileNotFoundException($"Module DLL not found: {dllPath} in {rootDirectory}");

            var assembly = Assembly.LoadFrom(dllPath);

            var moduleType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (moduleType == null)
            {
                logger.LogError("ERROR: Type Identity Mismatch in '{AssemblyName}'.", assembly.GetName().Name);
                logger.LogError("       The plugin implements 'IModule', but it creates a conflict with the Host's 'IModule'.");

                return (null, null);
            }

            if (Activator.CreateInstance(moduleType) is not IModule module)
                throw new Exception($"Failed to cast {moduleType.Name} to IModule.");

            var configBuilder = new ConfigurationBuilder();

            var baseConfigPath = Path.Combine(rootDirectory, $"{moduleName}.json");
            var envConfigPath = Path.Combine(rootDirectory, $"{moduleName}.{envName}.json");

            if (File.Exists(baseConfigPath))
            {
                configBuilder.AddJsonFile(baseConfigPath, optional: false, reloadOnChange: true);
                logger.LogInformation("Loaded config for '{ModuleName}' from '{ConfigFileName}'.", moduleName, Path.GetFileName(baseConfigPath));
            }

            if (File.Exists(envConfigPath))
            {
                configBuilder.AddJsonFile(envConfigPath, optional: true, reloadOnChange: true);
                logger.LogInformation("Loaded environment-specific config for '{ModuleName}' from '{ConfigFileName}'.", moduleName, Path.GetFileName(envConfigPath));
            }

            return (module, configBuilder.Build());
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to load module from '{ModuleName}'.", Path.GetFileName(moduleName));
            logger.LogError("         Reason: {Reason}", ex.Message);
            if (ex.InnerException != null)
                logger.LogError("         Inner: {Inner}", ex.InnerException.Message);

            return (null, null);
        }
    }
}