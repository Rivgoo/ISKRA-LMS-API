using Iskra.Bootstrapper.Extensions;

namespace Iskra.Api.Host;

/// <summary>
/// The main entry point for the Iskra Modular Platform.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Bootstrap the System (Load Modules, Register DI)
        var engine = builder.AddIskraPlatform();

        var app = builder.Build();

        // 2. Configure Pipeline (Middleware)
        engine.UseIskra(app);

        app.MapControllers();

        app.Run();
    }
}