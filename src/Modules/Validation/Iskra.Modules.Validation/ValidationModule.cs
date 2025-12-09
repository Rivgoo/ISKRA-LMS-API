using Iskra.Application.Abstractions.Validation;
using Iskra.Core.Contracts.Abstractions;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Validation.Abstractions.Services;
using Iskra.Modules.Validation.Emails;
using Iskra.Modules.Validation.Passwords;
using Iskra.Modules.Validation.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Iskra.Modules.Validation;

public class ValidationModule : IModule
{
    public string Name => "Iskra.Modules.Validation";

    public int Priority => 10;

    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration globalConfiguration, ILoggerFactory loggerFactory)
    {
        var config = globalConfiguration.GetSection(Name);

        // Bind Configuration
        services.Configure<UserValidationOptions>(config.GetSection("UserValidation"));
        services.Configure<PasswordValidationOptions>(config.GetSection("PasswordValidation"));
        services.Configure<EmailValidationOptions>(config.GetSection("EmailValidation"));

        // Register Granular Services
        services.AddScoped<IPasswordValidationService, PasswordValidationService>();
        services.AddScoped<IEmailValidationService, EmailValidationService>();

        // Register Entity Validator
        services.AddScoped<IValidationService<User>, UserValidationService>();
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }
}