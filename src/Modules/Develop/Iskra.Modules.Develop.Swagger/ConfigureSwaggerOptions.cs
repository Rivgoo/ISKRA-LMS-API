using Asp.Versioning.ApiExplorer;
using Iskra.Modules.Develop.Swagger.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Iskra.Modules.Develop.Swagger;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API Versioning to define the Swagger documents.</remarks>
internal class ConfigureSwaggerOptions(
    IApiVersionDescriptionProvider provider,
    IOptions<SwaggerOptions> settings)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly SwaggerOptions _settings = settings.Value;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = _settings.Title,
            Version = description.ApiVersion.ToString(),
            Description = _settings.Description,
            Contact = new OpenApiContact { Email = _settings.ContactEmail, Name = "Iskra Support" }
        };

        if (description.IsDeprecated)
            info.Description += " (This API version is deprecated)";

        return info;
    }
}