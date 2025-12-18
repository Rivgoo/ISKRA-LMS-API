using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace Iskra.Bootstrapper.Extensions;

internal static class DataProtectionExtensions
{
    /// <summary>
    /// Configures Data Protection to store keys in the database and encrypt them using a certificate.
    /// This ensures keys persist across deployments and are protected from database leaks.
    /// </summary>
    public static IServiceCollection AddIskraDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var dataProtection = services.AddDataProtection()
        .PersistKeysToDbContext<AppDbContextBase>()
        .SetApplicationName("Iskra.LMS");

        var certPath = configuration["DataProtection:CertificatePath"];
        var certPass = configuration["DataProtection:CertificatePassword"];

        if (!string.IsNullOrEmpty(certPath) && File.Exists(certPath))
        {
            try
            {
                var certificate = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPass);

                dataProtection.ProtectKeysWithCertificate(certificate);
                Log.Information("DataProtection keys will be encrypted with certificate: {Thumbprint}", certificate.Thumbprint);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load DataProtection certificate from {Path}. Keys will NOT be encrypted at rest.", certPath);
            }
        }
        else
        {
            Log.Warning("SECURITY WARNING: No DataProtection certificate configured. Keys are stored in PLAIN TEXT in the database.");
        }

        return services;
    }
}