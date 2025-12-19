using FluentValidation;
using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Users.Abstractions.Models.Requests;
using Iskra.Modules.Users.Abstractions.Repositories;
using Iskra.Modules.Users.Abstractions.Services;
using Iskra.Modules.Users.Managers;
using Iskra.Modules.Users.Options;
using Iskra.Modules.Users.Options.Seeding;
using Iskra.Modules.Users.Repositories;
using Iskra.Modules.Users.Services;
using Iskra.Modules.Users.Validation;
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

    public void RegisterServices(IServiceCollection services, IConfiguration globalConfig, ILoggerFactory loggerFactory)
    {
        var config = globalConfig.GetSection(Name);
        services.Configure<UserRegistrationOptions>(config.GetSection("Registration"));
        services.Configure<SeedingOptions>(config.GetSection("Seeding"));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IUserEntityService, UserEntityService>();

        // Managers
        services.AddScoped<IUserManager, UserManager>();

        // Validators
        services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();

        // Register Seeder
        services.AddTransient<IdentitySeeder>();
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }

    public async Task InitializeAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

        await seeder.SeedAsync();
    }
}