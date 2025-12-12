using Iskra.Core.Contracts.Abstractions;
using Iskra.Modules.Auth.Abstractions.Repositories;
using Iskra.Modules.Auth.Abstractions.Services;
using Iskra.Modules.Auth.Jobs;
using Iskra.Modules.Auth.Options;
using Iskra.Modules.Auth.Repositories;
using Iskra.Modules.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Iskra.Modules.Auth;

public class AuthModule : IModule
{
    public string Name => "Iskra.Modules.Auth";
    public int Priority => 1;

    public Assembly Assembly => Assembly.GetExecutingAssembly();

    public void RegisterServices(IServiceCollection services, IConfiguration globalConfig, ILoggerFactory loggerFactory)
    {
        // 1. Bind Options
        var moduleConfig = globalConfig.GetSection(Name);
        services.Configure<AuthOptions>(moduleConfig);

        // Get options synchronously for JwtBearer configuration
        var authOptions = moduleConfig.Get<AuthOptions>() ?? new AuthOptions();

        // 2. Register Core Services
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<ISessionService, SessionService>();

        // Internal Services (Not exposed in Abstractions)
        services.AddScoped<JwtProvider>();
        services.AddScoped<ICookieService, CookieService>();

        // Register Hosted Service
        services.AddHostedService<SessionCleanupJob>();

        // Required for Cookie Access
        services.AddHttpContextAccessor();

        // 3. Configure Authentication (JWT from Cookie)
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions.Jwt.Issuer,
                    ValidAudience = authOptions.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Jwt.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                // Read Access Token from Cookie
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var cookieName = authOptions.Cookies.AccessTokenName;
                        context.Token = context.Request.Cookies[cookieName];
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
    }

    public void ConfigureMiddleware(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}