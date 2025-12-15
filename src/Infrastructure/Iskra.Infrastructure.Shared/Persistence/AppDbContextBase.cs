using Iskra.Core.Contracts.Persistence;
using Iskra.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Infrastructure.Shared.Persistence;

/// <summary>
/// An abstract base context that handles configuration discovery.
/// Concrete providers (MariaDb, Postgres) will inherit from this.
/// </summary>
public abstract class AppDbContextBase : DbContext
{
    protected AppDbContextBase(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Automatically updates IAuditable properties.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateInterceptors();
        return base.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// Automatically updates IAuditable properties.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateInterceptors();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateInterceptors()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                if (entry.Entity is IAuditable auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedAt = DateTimeOffset.UtcNow;
                        auditable.UpdatedAt = null;
                    }
                    else
                    {
                        auditable.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }

                if (entry.Entity is IConcurrent concurrent)
                    concurrent.ConcurrencyToken = Guid.NewGuid();
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IConcurrent).IsAssignableFrom(entityType.ClrType))
            {
                var tokenProperty = entityType.FindProperty(nameof(IConcurrent.ConcurrencyToken));

                if (tokenProperty != null)
                    tokenProperty.IsConcurrencyToken = true;
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContextBase).Assembly);

        // Dynamic Module Loading
        var moduleConfigurations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModelConfiguration).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IModelConfiguration>();

        foreach (var config in moduleConfigurations)
            config.Configure(modelBuilder);
    }
}