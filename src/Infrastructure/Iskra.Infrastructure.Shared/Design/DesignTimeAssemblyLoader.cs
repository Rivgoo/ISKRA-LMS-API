using System.Reflection;

namespace Iskra.Infrastructure.Shared.Design;

public static class DesignTimeAssemblyLoader
{
    public static void LoadModules()
    {
        var modulesPath = FindModulesDirectory();

        if (!Directory.Exists(modulesPath))
        {
            Console.WriteLine($"[Design-Time] Modules folder not found at: {modulesPath}. Build the solution first.");
            return;
        }

        var dlls = Directory.GetFiles(modulesPath, "Iskra.Modules.*.dll");
        Console.WriteLine($"[Design-Time] Scanning {dlls.Length} modules in {modulesPath}...");

        foreach (var dll in dlls)
        {
            try
            {
                Assembly.LoadFrom(dll);
                Console.WriteLine($"[Design-Time] Loaded: {Path.GetFileName(dll)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Design-Time] Failed to load {Path.GetFileName(dll)}: {ex.Message}");
            }
        }
    }

    private static string? FindModulesDirectory()
    {
        // 1. Check if we are running from the build output directly
        var currentBase = AppDomain.CurrentDomain.BaseDirectory;
        var localModules = Path.Combine(currentBase, "Modules");
        if (Directory.Exists(localModules)) return localModules;

        // 2. Recursive search upwards for the 'build/Modules' convention (Dev Environment)
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir != null)
        {
            var target = Path.Combine(dir.FullName, "build", "Modules");
            if (Directory.Exists(target)) return target;

            dir = dir.Parent;
        }

        return null;
    }
}