using Lumora.Domain.Entities.Event;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class EventConfiguration
{
    public static void ConfigureEvent(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Event>();

        //key
        entity.HasKey(e => e.Id);

        //properties
        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(e => e.EventDate)
            .IsRequired();

        entity.Property(e => e.Budget)
            .IsRequired();

        entity.Property(e => e.Duration)
            .IsRequired();

        entity.Property(e => e.PhotographyStyle)
            .HasMaxLength(200);

        entity.Property(e => e.SpecialRequirements)
            .HasMaxLength(500);

        entity.OwnsOne(e => e.Location, eo =>
        {
            eo.ToJson();
        });

        entity.HasOne(e => e.Consumer)
            .WithMany()
            .HasForeignKey(e => e.ConsumerId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.Inquiries)
            .WithOne(i => i.Event)
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.SelectedStudio)
            .WithMany()
            .HasForeignKey(e => e.SelectedStudioId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasMany(e => e.EventTags)
           .WithOne(et => et.Event)
           .HasForeignKey(et => et.EventId)
           .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.ConsumerId);
        entity.HasIndex(e => e.Status);


        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt == null);


    }
}
