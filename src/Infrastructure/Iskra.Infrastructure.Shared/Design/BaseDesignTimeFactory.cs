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

        // Locate the CENTRALIZED config file in the build folder
        var buildPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../build"));

        if (!Directory.Exists(buildPath))
            throw new DirectoryNotFoundException($"Build directory not found at {buildPath}. Did you build the Host?");

        // Load appsettings.json
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(buildPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true);

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
}