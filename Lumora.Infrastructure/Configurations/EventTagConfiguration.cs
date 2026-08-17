using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Tag;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class EventTagConfiguration
{
    public static void ConfigureEventTag(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EventTag>();

        // Composite key prevents duplicate tag associations
        entity.HasKey(et => new { et.EventId, et.TagId });

        // Event -> EventTag (one-to-many)
        entity.HasOne(et => et.Event)
            .WithMany(e => e.EventTags)
            .HasForeignKey(et => et.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tag -> EventTag (one-to-many)
        entity.HasOne(et => et.Tag)
            .WithMany(t => t.EventTags)  // Explicitly specify back reference
            .HasForeignKey(et => et.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Performance indexes
        entity.HasIndex(et => et.TagId);
        entity.HasIndex(et => et.EventId);

        // Soft delete support
        entity.HasQueryFilter(et => et.IsActive && et.DeletedAt == null);
    }
}
