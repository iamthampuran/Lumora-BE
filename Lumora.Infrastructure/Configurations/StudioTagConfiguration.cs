using Lumora.Domain.Entities.Studio;
using Lumora.Domain.Entities.Tag;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class StudioTagConfiguration
{
    public static void ConfigureStudioTag(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<StudioTag>();

        // Composite key prevents duplicate tag associations
        entity.HasKey(st => new { st.StudioProfile, st.TagId });

        // StudioProfile -> StudioTag (one-to-many)
        entity.HasOne(st => st.Studio)
            .WithMany(sp => sp.Tags)
            .HasForeignKey(st => st.StudioProfile)
            .OnDelete(DeleteBehavior.Cascade);

        // Tag -> StudioTag (one-to-many)
        entity.HasOne(st => st.Tag)
            .WithMany(t => t.StudioTags)
            .HasForeignKey(st => st.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Performance indexes
        entity.HasIndex(st => st.TagId);
        entity.HasIndex(st => st.StudioProfile);

        // Soft delete support
        entity.HasQueryFilter(st => st.IsActive && st.DeletedAt == null);
    }
}
