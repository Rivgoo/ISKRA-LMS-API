using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Abstractions.Security;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Iskra.Modules.Users.Abstractions.Repositories;
using Iskra.Modules.Users.Options.Seeding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Users.Services;

/// <summary>
/// Seeds initial roles, permissions, and users into the database.
/// This service bridges the gap between IAM and Users modules during startup.
/// </summary>
internal sealed class IdentitySeeder(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IUserRoleRepository userRoleRepository,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IOptions<SeedingOptions> options,
    ILogger<IdentitySeeder> logger)
{
    private readonly SeedingOptions _options = options.Value;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (_options.Roles.Count == 0 && _options.Users.Count == 0) return;

        logger.LogInformation("Starting Identity Seeding...");

        try
        {
            await SeedRolesAsync(ct);
            await SeedUsersAsync(ct);
            logger.LogInformation("Identity Seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Identity Seeding failed.");
            throw;
        }
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        foreach (var seedRole in _options.Roles)
        {
            var existingRole = await roleRepository.GetByNameAsync(seedRole.Name, ct);

            if (existingRole == null)
            {
                logger.LogInformation("Creating Role: {RoleName} (System: {IsSystem})", seedRole.Name, seedRole.IsSystem);

                var newRole = new Role(seedRole.Name, seedRole.Description, seedRole.IsSystem);

                roleRepository.Add(newRole);

                if (seedRole.Permissions.Count > 0)
                    await permissionRepository.ReplacePermissionsAsync(newRole.Id, seedRole.Permissions, ct);
            }
            else if (existingRole.IsSystem)
            {
                logger.LogInformation("Updating System Role Permissions: {RoleName}", seedRole.Name);
                await permissionRepository.ReplacePermissionsAsync(existingRole.Id, seedRole.Permissions, ct);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task SeedUsersAsync(CancellationToken ct)
    {
        foreach (var seedUser in _options.Users)
        {
            var exists = await userRepository.GetByEmailAsync(seedUser.Email, ct);
            if (exists != null) continue;

            // Find Role (Must exist now)
            var role = await roleRepository.GetByNameAsync(seedUser.Role, ct);
            if (role == null)
            {
                logger.LogWarning("Skipping seed user '{Email}': Role '{Role}' not found.", seedUser.Email, seedUser.Role);
                continue;
            }

            logger.LogInformation("Creating Seed User: {Email}", seedUser.Email);

            var user = User.Create(
                seedUser.Email,
                seedUser.FirstName,
                seedUser.LastName,
                null,
                passwordHasher.Hash(seedUser.Password),
                true // IsEmailConfirmed
            );


            userRepository.Add(user);

            userRoleRepository.Add(new UserRole(user.Id, role.Id));
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}