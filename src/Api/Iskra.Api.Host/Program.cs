using Iskra.Bootstrapper.Extensions;

namespace Iskra.Api.Host;

/// <summary>
/// The main entry point for the Iskra Modular Platform.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Bootstrap the System (Load Modules, Register DI)
        var engine = builder.AddIskraPlatform();

        var app = builder.Build();

        // 2. Configure Pipeline (Middleware)
        engine.UseIskra(app);

        // 3. Async Initialization
        await engine.InitializeModulesAsync(app);

        app.MapControllers();

        app.Run();
    }
}