using Lumora.Domain.Entities.Tag;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class TagConfiguration
{
    public static void ConfigureTag(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Tag>();

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasIndex(e => e.Name)
            .IsUnique();

        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt != null);
    }
}
