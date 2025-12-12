using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Users.Abstractions.Repositories;
using Iskra.Modules.Users.Abstractions.Services;
using Iskra.Modules.Users.Repositories;
using Iskra.Modules.Users.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Modules.Users;

public class UsersModule : IModule
{
    public string Name => "Iskra.Modules.Users";
    public int Priority => 10;
    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration config, ILoggerFactory loggerFactory)
    {
        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        // Register Services
        services.AddScoped<IUserEntityService, UserEntityService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
    }

    public void ConfigureMiddleware(WebApplication app) { }
}