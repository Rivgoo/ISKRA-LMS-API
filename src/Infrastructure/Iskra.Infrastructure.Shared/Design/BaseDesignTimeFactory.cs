using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Iskra.Infrastructure.Shared.Design;

public abstract class BaseDesignTimeFactory<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : AppDbContextBase
{
    protected abstract string ConfigFileName { get; }
    protected abstract string ConnectionStringName { get; }

    public TContext CreateDbContext(string[] args)
    {
        // 1. Load Modules into AppDomain so EF Core can find their Entities
        DesignTimeAssemblyLoader.LoadModules();

        // 2. Load Configuration
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ConfigFileName, optional: false)
            .AddJsonFile(ConfigFileName.Replace(".json", ".Development.json"), optional: true);

        var config = configBuilder.Build();
        var connectionString = config.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string '{ConnectionStringName}' missing in {ConfigFileName}");

        var optionsBuilder = CreateOptionsBuilder(connectionString);
        return CreateContext(optionsBuilder.Options);
    }

    protected abstract DbContextOptionsBuilder<TContext> CreateOptionsBuilder(string connectionString);
    protected abstract TContext CreateContext(DbContextOptions<TContext> options);
}