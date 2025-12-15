using Iskra.Core.Contracts.Constants;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.Loader;

namespace Iskra.Bootstrapper.PluginManagement;

/// <summary>
/// A centralized resolver that helps the CLR find assemblies located in the 'Modules' directory.
/// This works within the Default Load Context, ensuring strict type compatibility across all modules.
/// </summary>
internal static class GlobalModuleResolver
{
    private static ILogger? _logger;
    private static string? _modulesPath;
    private static bool _isInitialized;

    public static void Initialize(ILogger logger)
    {
        if (_isInitialized) return;

        _logger = logger;
        _modulesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConstants.ModulesRootPath);

        if (!Directory.Exists(_modulesPath))
        {
            _logger.LogWarning("Modules directory not found at: {Path}. Resolver disabled.", _modulesPath);
            return;
        }

        AssemblyLoadContext.Default.Resolving += ResolveAssembly;

        _isInitialized = true;
        _logger.LogInformation("Global Module Resolver initialized. Watching: {Path}", _modulesPath);
    }

    private static Assembly? ResolveAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        if (assemblyName.Name is null || assemblyName.Name.EndsWith(".resources"))
            return null;

        var dllPath = Path.Combine(_modulesPath!, $"{assemblyName.Name}.dll");

        if (File.Exists(dllPath))
        {
            try
            {
                return context.LoadFromAssemblyPath(dllPath);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Failed to load dependency '{Name}': {Error}", assemblyName.Name, ex.Message);
            }
        }

        return null;
    }
}