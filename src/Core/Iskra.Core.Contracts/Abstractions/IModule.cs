using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Core.Contracts.Abstractions;

/// <summary>
/// Represents a self-contained unit of functionality that can be loaded dynamically.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Gets the unique name of the module for identification and logging.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the execution priority for the middleware pipeline.
    /// Lower values are executed earlier in the request lifecycle.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the assembly containing the module logic.
    /// Required for the Host to discover Controllers and Validators within the DLL.
    /// </summary>
    Assembly Assembly { get; }

    /// <summary>
    /// Registers dependencies and services into the DI container.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">The isolated configuration specific to this module.</param>
    /// <param name="loggerFactory">A factory to create loggers for services during registration.</param>
    void RegisterServices(IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory);

    /// <summary>
    /// Configures the HTTP request pipeline (middleware) for this module.
    /// </summary>
    /// <param name="app">The web application builder.</param>
    void ConfigureMiddleware(WebApplication app);

    /// <summary>
    /// Performs asynchronous initialization tasks (e.g., DB Migrations, Seeding).
    /// This is called after the app is built but before it starts running.
    /// </summary>
    Task InitializeAsync(WebApplication app);
}