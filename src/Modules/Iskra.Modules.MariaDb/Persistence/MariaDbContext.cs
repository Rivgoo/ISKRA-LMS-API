using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Iskra.Modules.MariaDb.Persistence;

internal class MariaDbContext(DbContextOptions<MariaDbContext> options) : AppDbContextBase(options)
{
    /// <summary>
    /// Configures global conventions for data types.
    /// </summary>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Applies default configs from Shared Kernel & Dynamic Modules
        base.OnModelCreating(modelBuilder);

        // 2. Apply MariaDB Naming Conventions (Snake_Case)
        ApplySnakeCaseConvention(modelBuilder);

        // 3. Load Provider-Specific Overrides (if any exist in this project)
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void ApplySnakeCaseConvention(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convert Table Names: "UserRoles" -> "user_roles"
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                // Convert Column Names: "FirstName" -> "first_name"
                var columnName = property.GetColumnName();

                if (!string.IsNullOrEmpty(columnName))
                    property.SetColumnName(ToSnakeCase(columnName));
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var startUnderscores = input.TakeWhile(c => c == '_').Count();
        var endUnderscores = input.Reverse().TakeWhile(c => c == '_').Count();
        var core = input.Substring(startUnderscores, input.Length - startUnderscores - endUnderscores);

        var converted = string.Concat(core.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();

        return new string('_', startUnderscores) + converted + new string('_', endUnderscores);
    }
}