using FluentValidation;
using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Iam.Abstractions.Models;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Iskra.Modules.Iam.Abstractions.Services;
using Iskra.Modules.Iam.Repositories;
using Iskra.Modules.Iam.Services;
using Iskra.Modules.Iam.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Modules.Iam;

public class IamModule : IModule
{
    public string Name => "Iskra.Modules.Iam";
    public int Priority => 1;

    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration config, ILoggerFactory loggerFactory)
    {
        // 1. Repositories
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        // 2. Services
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IPermissionService, PermissionService>();

        // 3. Validation
        services.AddScoped<IValidator<CreateRoleRequest>, CreateRoleRequestValidator>();
        services.AddScoped<IValidator<UpdateRoleRequest>, UpdateRoleRequestValidator>();
        services.AddScoped<IValidator<UpdateRolePermissionsRequest>, UpdateRolePermissionsRequestValidator>();
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }
}