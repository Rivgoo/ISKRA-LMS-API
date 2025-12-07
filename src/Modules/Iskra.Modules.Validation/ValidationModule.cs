using Iskra.Application.Abstractions.Validation;
using Iskra.Core.Contracts.Abstractions;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Validation.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Modules.Validation;

public class ValidationModule : IModule
{
	public string Name => "Modules.Validation";

	public int Priority => 10;

	public Assembly Assembly => Assembly.GetExecutingAssembly();

	public void RegisterServices(IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        services.Configure<UserValidationOptions>(configuration.GetSection("UserValidation"));
        services.AddScoped<IValidationService<User>, UserValidationService>();
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }
}