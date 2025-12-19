using Iskra.Application.Filters.Abstractions;
using Iskra.Application.Filters.Abstractions.Options;
using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Filters.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
namespace Iskra.Modules.Filters;

public class FiltersModule : IModule
{
    public string Name => "Iskra.Modules.Filters";
    public int Priority => 5;
    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        var section = configuration.GetSection(Name);
        services.Configure<FilterSettingsOptions>(section);

        services.AddScoped(typeof(IDataQueryService<,>), typeof(DataQueryService<,>));
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }

    public Task InitializeAsync(WebApplication app)
    {
        return Task.CompletedTask;
    }
}