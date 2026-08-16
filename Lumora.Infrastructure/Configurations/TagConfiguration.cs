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

        // Studio-Tag: Many-to-many via implicit junction table
        entity.HasMany(t => t.Studios)
            .WithMany(s => s.Tags)
            .UsingEntity("StudioTags");

        // Event-Tag: One-to-many via explicit EventTag association
        entity.HasMany(t => t.EventTags)
            .WithOne(et => et.Tag)
            .HasForeignKey(et => et.TagId);

        // Soft delete support
        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt == null);
    }
}
