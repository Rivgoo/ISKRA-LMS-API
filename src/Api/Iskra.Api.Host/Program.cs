using Iskra.Bootstrapper.Extensions;
using Serilog;

namespace Iskra.Api.Host;

/// <summary>
/// The main entry point for the Iskra Modular Platform.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();
        try
        {
            Log.Information("Starting Iskra LMS Host...");
            var builder = WebApplication.CreateBuilder(args);

            // 1. Bootstrap the System (Load Modules, Register DI)
            var engine = builder.AddIskraPlatform();

            var app = builder.Build();

            // 2. Configure Pipeline (Middleware)
            engine.UseIskra(app);

            // 3. Async Initialization
            await engine.InitializeModulesAsync(app);

            // 4. Request Logging
            app.UseSerilogRequestLogging();

            app.MapControllers();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly during startup.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}