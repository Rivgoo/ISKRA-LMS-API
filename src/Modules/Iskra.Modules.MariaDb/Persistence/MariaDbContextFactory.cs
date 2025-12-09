using Iskra.Infrastructure.Shared.Design;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.MariaDb.Persistence;

/// <summary>
/// Factory used by EF Core CLI tools to create the context at design time (migrations).
/// </summary>
internal class MariaDbContextFactory : BaseDesignTimeFactory<MariaDbContext>
{
    protected override string ModuleConfigSectionName => "Iskra.Modules.MariaDb";
    protected override string ConnectionStringName => "DefaultConnection";

    protected override DbContextOptionsBuilder<MariaDbContext> CreateOptionsBuilder(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<MariaDbContext>();

        builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
            b => b.MigrationsAssembly(GetType().Assembly.FullName));

        return builder;
    }

    protected override MariaDbContext CreateContext(DbContextOptions<MariaDbContext> options)
    {
        return new MariaDbContext(options);
    }
}