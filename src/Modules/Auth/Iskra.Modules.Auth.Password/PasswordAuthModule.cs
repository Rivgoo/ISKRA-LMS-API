using FluentValidation;
using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Auth.Password.Abstractions.Requests;
using Iskra.Modules.Auth.Password.Abstractions.Services;
using Iskra.Modules.Auth.Password.Services;
using Iskra.Modules.Auth.Password.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Modules.Auth.Password;

public class PasswordAuthModule : IModule
{
    public string Name => "Iskra.Modules.Auth.Password";
    public int Priority => 10;

    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration config, ILoggerFactory loggerFactory)
    {
        services.AddScoped<IPasswordLoginService, PasswordLoginService>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }

    public Task InitializeAsync(WebApplication app) => Task.CompletedTask;
}