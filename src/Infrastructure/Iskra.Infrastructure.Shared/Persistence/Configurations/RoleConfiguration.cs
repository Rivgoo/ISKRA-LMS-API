using Iskra.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iskra.Infrastructure.Shared.Persistence.Configurations;

/// <summary>
/// Configures the database schema for the Role entity.
/// </summary>
internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(500);
    }
}