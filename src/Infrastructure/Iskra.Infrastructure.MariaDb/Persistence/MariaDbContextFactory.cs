using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Iskra.Infrastructure.MariaDb.Persistence;

/// <summary>
/// Factory used by EF Core CLI tools to create the context at design time (migrations).
/// </summary>
internal class MariaDbContextFactory : IDesignTimeDbContextFactory<MariaDbContext>
{
    public MariaDbContext CreateDbContext(string[] args)
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Iskra.Infrastructure.MariaDb.json", optional: false)
            .AddJsonFile("Iskra.Infrastructure.MariaDb.Development.json", optional: true);

        var config = configBuilder.Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string not found in design-time configuration.");

        var optionsBuilder = new DbContextOptionsBuilder<MariaDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
            b => b.MigrationsAssembly(GetType().Assembly.FullName));

        return new MariaDbContext(optionsBuilder.Options);
    }
}