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
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// Automatically updates IAuditable properties.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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