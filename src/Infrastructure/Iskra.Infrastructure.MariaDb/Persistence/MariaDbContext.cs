using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Infrastructure.MariaDb.Persistence;

/// <summary>
/// Concrete implementation of the context for MariaDB.
/// </summary>
internal class MariaDbContext : AppDbContextBase
{
    public MariaDbContext(DbContextOptions<MariaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}