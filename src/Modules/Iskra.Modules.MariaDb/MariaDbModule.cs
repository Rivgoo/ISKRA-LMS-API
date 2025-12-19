using Iskra.Core.Contracts.Abstractions;
using Iskra.Infrastructure.Shared;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Infrastructure.Shared.Search;
using Iskra.Modules.MariaDb.Persistence;
using Iskra.Modules.MariaDb.Search;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Modules.MariaDb;

/// <summary>
/// The infrastructure module responsible for MariaDB connectivity.
/// </summary>
public class MariaDbModule : IModule
{
    private IConfiguration? _moduleConfiguration;
    public string Name => "Iskra.Modules.MariaDb";

    public int Priority => 1;

    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration globalConfiguration, ILoggerFactory loggerFactory)
    {
        var configuration = globalConfiguration.GetSection(Name);

        _moduleConfiguration = configuration;
        var logger = loggerFactory.CreateLogger<MariaDbModule>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Fatal: 'DefaultConnection' is missing in {Name}.json");

        ServerVersion serverVersion;
        try
        {
            serverVersion = ServerVersion.AutoDetect(connectionString);
            logger.LogInformation($"Successfully connected. Server Version detected.");
        }
        catch (Exception ex)
        {
            logger.LogError("ERROR: Could not connect to database to detect version. \nReason: {ErrorMessage}\n Fallback: Using default version (MariaDB 10.5).", ex.Message);

            serverVersion = new MariaDbServerVersion(new Version(10, 5));
        }

        services.AddDbContext<AppDbContextBase, MariaDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion,
                b =>
                {
                    b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    b.EnableRetryOnFailure(
                            maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(1),
                                errorNumbersToAdd: null);
                });
        });

        logger.LogInformation($"Database Context registered.");

        services.AddSingleton<ISearchProvider, MariaDbSearchProvider>();

        services.AddSharedInfrastructure();

        logger.LogInformation($"Infrastructure persistence services registered.");
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }

    public async Task InitializeAsync(WebApplication app)
        => await ApplyMigrations(app);

    private async Task ApplyMigrations(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContextBase>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MariaDbModule>>();

        if(_moduleConfiguration == null)
            throw new InvalidOperationException("Module configuration is not initialized.");

        var autoMigrate = _moduleConfiguration.GetValue<bool>("DatabaseSettings:AutoMigrate");

        if (!autoMigrate)
        {
            logger.LogInformation("Auto-migrations are disabled. Skipping migration step.");
            return;
        }

        try
        {
            logger.LogInformation("Applying pending migrations (Async)...");
            await db.Database.MigrateAsync();
            logger.LogInformation($"Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError($"[FATAL] Migration failed.");
            logger.LogError("Error: {ErrorMessage}", ex.Message);
        }
    }
}