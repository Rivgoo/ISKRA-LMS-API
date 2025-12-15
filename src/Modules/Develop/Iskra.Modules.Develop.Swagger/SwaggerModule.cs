using Asp.Versioning.ApiExplorer;
using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Develop.Swagger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Iskra.Modules.Develop.Swagger;

public class SwaggerModule : IModule
{
    public string Name => "Iskra.Modules.Develop.Swagger";

    public int Priority => 100;

    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration globalConfig, ILoggerFactory loggerFactory)
    {
        var config = globalConfig.GetSection(Name);
        services.Configure<SwaggerOptions>(config);

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        var authCookieName = globalConfig["Iskra.Modules.Auth:Cookies:AccessTokenName"] ?? "iskra_access";

        services.AddSwaggerGen(options =>
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var xmlFiles = Directory.GetFiles(baseDir, "Iskra.*.xml");

            foreach (var xmlFile in xmlFiles)
                options.IncludeXmlComments(xmlFile);

            // Define Security Scheme (Cookie)
            // Even though browser sends cookie automatically, we define it here for documentation
            options.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = authCookieName,
                Description = "JWT Access Token in HttpOnly Cookie. Login via /api/v1/auth/login to set it."
            });

            // Make sure Swagger knows endpoints require this scheme
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "cookieAuth" }
                    },
                    []
                }
            });
        });
    }

    public void ConfigureMiddleware(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            var logger = app.Services.GetRequiredService<ILogger<SwaggerModule>>();
            logger.LogWarning("Swagger is enabled only in Development environment. Current environment: {Environment}. Swagger middleware will not be registered.", app.Environment.EnvironmentName);
            return;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
    }

    public Task InitializeAsync(WebApplication app) => Task.CompletedTask;
}