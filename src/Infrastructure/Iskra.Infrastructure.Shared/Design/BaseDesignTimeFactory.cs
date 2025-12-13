using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Iskra.Infrastructure.Shared.Design;

public abstract class BaseDesignTimeFactory<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : AppDbContextBase
{
    protected abstract string ModuleConfigSectionName { get; }
    protected abstract string ConnectionStringName { get; }

    public TContext CreateDbContext(string[] args)
    {
        // Load Modules into AppDomain so EF Core can find their Entities
        DesignTimeAssemblyLoader.LoadModules();

        // Resolve Configuration Path
        var configPath = FindConfigurationPath();

        // Load appsettings.json
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(configPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables();

        var globalConfig = configBuilder.Build();

        var moduleConfig = globalConfig.GetSection(ModuleConfigSectionName);
        var connectionString = moduleConfig.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string '{ConnectionStringName}' missing in section '{ModuleConfigSectionName}' of appsettings.json");

        var optionsBuilder = CreateOptionsBuilder(connectionString);
        return CreateContext(optionsBuilder.Options);
    }

    protected abstract DbContextOptionsBuilder<TContext> CreateOptionsBuilder(string connectionString);
    protected abstract TContext CreateContext(DbContextOptions<TContext> options);

    /// <summary>
    /// Locates the directory containing appsettings.json.
    /// Priority: Environment Variable -> Current Directory -> Host Project Source -> Build Output.
    /// </summary>
    private string FindConfigurationPath()
    {
        // A. Explicit Override (CI/CD)
        var envPath = Environment.GetEnvironmentVariable("ISKRA_CONFIG_PATH");
        if (!string.IsNullOrEmpty(envPath) && Directory.Exists(envPath))
            return envPath;

        // B. Current Directory (if running from output)
        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")))
            return Directory.GetCurrentDirectory();

        // C. Search Upwards for Source Code (Dev Environment)
        // Look for the Api.Host folder which holds the master config
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir != null)
        {
            // Check for standard source structure
            var hostConfigPath = Path.Combine(dir.FullName, "src", "Api", "Iskra.Api.Host");
            if (Directory.Exists(hostConfigPath) && File.Exists(Path.Combine(hostConfigPath, "appsettings.json")))
                return hostConfigPath;

            // Check for 'build' folder convention
            var buildPath = Path.Combine(dir.FullName, "build");
            if (Directory.Exists(buildPath) && File.Exists(Path.Combine(buildPath, "appsettings.json")))
                return buildPath;

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate appsettings.json. Ensure you are in the project root or set ISKRA_CONFIG_PATH.");
    }
}