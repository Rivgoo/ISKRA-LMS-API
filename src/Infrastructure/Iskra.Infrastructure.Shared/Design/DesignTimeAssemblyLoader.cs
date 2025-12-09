using System.Reflection;

namespace Iskra.Infrastructure.Shared.Design;

public static class DesignTimeAssemblyLoader
{
    public static void LoadModules()
    {
        var rootDir = FindSolutionRoot(Directory.GetCurrentDirectory());
        if (rootDir == null)
        {
            Console.WriteLine("Warning: Solution root not found.");
            return;
        }

        var modulesPath = Path.Combine(rootDir, "build", "Modules");

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

    private static string? FindSolutionRoot(string path)
    {
        var dir = new DirectoryInfo(path);
        while (dir != null)
        {
            // Look for the "build" folder or .sln file as anchor
            if (Directory.Exists(Path.Combine(dir.FullName, "build")) || dir.GetFiles("*.sln").Length > 0)
                return dir.FullName;

            dir = dir.Parent;
        }
        return null;
    }
}