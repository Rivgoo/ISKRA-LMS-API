using Iskra.Bootstrapper.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Iskra.Bootstrapper.Security;

internal static class CorsConfigurator
{
    public static void ConfigureCors(IServiceCollection services, SecurityOptions options)
    {
        var policies = options.Cors.Policies;

        if (policies.Count == 0) return;

        services.AddCors(corsOptions =>
        {
            foreach (var policyConfig in policies)
            {
                corsOptions.AddPolicy(policyConfig.Name, builder =>
                {
                    // Origins
                    if (policyConfig.Origins.Contains("*"))
                        builder.AllowAnyOrigin();
                    else
                        builder.WithOrigins([.. policyConfig.Origins]);

                    // Methods
                    if (policyConfig.Methods.Contains("*"))
                        builder.AllowAnyMethod();
                    else
                        builder.WithMethods([.. policyConfig.Methods]);

                    // Headers
                    if (policyConfig.Headers.Contains("*"))
                        builder.AllowAnyHeader();
                    else
                        builder.WithHeaders([.. policyConfig.Headers]);

                    // Credentials
                    if (policyConfig.AllowCredentials && !policyConfig.Origins.Contains("*"))
                        builder.AllowCredentials();
                });
            }
        });
    }

    public static void UseConfiguredCors(WebApplication app, SecurityOptions options)
    {
        var defaultPolicy = options.Cors.DefaultPolicyName;

        if (!string.IsNullOrEmpty(defaultPolicy))
            app.UseCors(defaultPolicy);
    }
}