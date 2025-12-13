using Iskra.Core.Contracts.Constants;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Bootstrapper.PluginManagement;

/// <summary>
/// Handles the resolution of assemblies that are not found in the host's base directory.
/// Redirects the runtime to look into the Modules directory.
/// </summary>
internal class AssemblyResolver
{
    private static string? _modulesDirectory;
    private static bool _isInitialized;
    private static ILogger<AssemblyResolver>? _logger;

    /// <summary>
    /// Initializes the resolver and subscribes to the AppDomain.AssemblyResolve event.
    /// </summary>
    public static void Initialize(ILogger<AssemblyResolver> logger)
    {
        if (_isInitialized) return;

        var modulesDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            PathConstants.ModulesRootPath));

        _logger = logger;

        if (!Directory.Exists(modulesDirectory))
            throw new DirectoryNotFoundException($"Modules directory not found: {modulesDirectory}");

        _modulesDirectory = modulesDirectory;
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        _isInitialized = true;

        logger.LogInformation("Initialized. Watching directory: {ModulesDirectory}", _modulesDirectory);
    }

    private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name).Name;
        if (string.IsNullOrEmpty(assemblyName) || _modulesDirectory == null) return null;

        var dllPath = Path.Combine(_modulesDirectory, $"{assemblyName}.dll");

        if (File.Exists(dllPath))
        {
            try
            {
                return Assembly.LoadFrom(dllPath);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Error loading '{AssemblyName}': {ErrorMessage}", assemblyName, ex.Message);
            }
        }

        // Return null to let CLR try other resolution methods or fail
        return null;
    }
}