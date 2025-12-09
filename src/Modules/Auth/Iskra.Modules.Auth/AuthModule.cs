using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Auth.Abstractions.Repositories;
using Iskra.Modules.Auth.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Modules.Auth;

public class AuthModule : IModule
{
    public string Name => "Modules.Auth";
    public int Priority => 5; 
    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration config, ILoggerFactory loggerFactory)
    {
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }

    public void ConfigureMiddleware(WebApplication app) { }
}