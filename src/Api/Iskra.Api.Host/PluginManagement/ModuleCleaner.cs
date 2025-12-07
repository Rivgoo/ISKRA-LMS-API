using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Iskra.Api.Host.PluginManagement;

internal class ModuleCleaner
{
    private static readonly string[] _protectedPrefixes =
    {
        "Microsoft.EntityFrameworkCore.Design",
        "Microsoft.EntityFrameworkCore.Tools",
        "Microsoft.CodeAnalysis",
        "Humanizer"
    };

    public static void Clean(string modulesPath, string[] enabledModules, ILogger<ModuleCleaner> logger)
    {
        var absolutePath = Path.GetFullPath(modulesPath);
        if (!Directory.Exists(absolutePath)) return;

        logger.LogInformation("Analyzing modules in {Path}", absolutePath);

        var allowedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var processingQueue = new Queue<string>();

        foreach (var moduleName in enabledModules)
        {
            var dllPath = Path.Combine(absolutePath, $"{moduleName}.dll");

            if (File.Exists(dllPath) && allowedFiles.Add(dllPath))
                processingQueue.Enqueue(dllPath);
        }

        while (processingQueue.Count > 0)
        {
            var currentFilePath = processingQueue.Dequeue();
            var dependencies = GetReferencesNonLocking(currentFilePath, absolutePath);

            foreach (var dependencyPath in dependencies)
                if (allowedFiles.Add(dependencyPath))
                    processingQueue.Enqueue(dependencyPath);
        }

        var allFiles = Directory.GetFiles(absolutePath, "*.*");
        var deletedCount = 0;

        foreach (var file in allFiles)
        {
            var extension = Path.GetExtension(file).ToLowerInvariant();
            var fileNameNoExt = Path.GetFileNameWithoutExtension(file);
            var expectedDllPath = Path.Combine(absolutePath, $"{fileNameNoExt}.dll");

            if (_protectedPrefixes.Any(prefix => fileNameNoExt.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                continue;

            if (allowedFiles.Contains(expectedDllPath))
            {
                allowedFiles.Add(file);
            }
            else if (!allowedFiles.Contains(file))
            {
                try
                {
                    File.Delete(file);
                    logger.LogInformation("Deleted unused file: {FileName}", Path.GetFileName(file));
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError("Could not delete '{FileName}': {ErrorMessage}", Path.GetFileName(file), ex.Message);
                }
            }
        }

        if (deletedCount > 0)
            logger.LogWarning("Cleanup complete. Removed {DeletedCount} files.", deletedCount);
    }

    private static IEnumerable<string> GetReferencesNonLocking(string filePath, string searchDirectory)
    {
        var references = new List<string>();
        try
        {
            using var stream = File.OpenRead(filePath);
            using var peReader = new PEReader(stream);

            if (!peReader.HasMetadata) return references;

            var metadataReader = peReader.GetMetadataReader();

            foreach (var handle in metadataReader.AssemblyReferences)
            {
                var reference = metadataReader.GetAssemblyReference(handle);
                var name = metadataReader.GetString(reference.Name);
                var expectedPath = Path.Combine(searchDirectory, $"{name}.dll");

                if (File.Exists(expectedPath))
                    references.Add(expectedPath);
            }
        }
        catch { /* Ignore non-managed DLLs */ }
        return references;
    }
}