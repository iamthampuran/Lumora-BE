using Lumora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class ConsumerProfileConfiguration
{
    public static void ConfigureConsumerProfile(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ConsumerProfile>();

        //key
        entity.HasKey(x => x.Id);

        //properties
        entity.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(e => e.Phone)
            .HasMaxLength(20);

        entity.Property(e => e.Bio)
            .HasMaxLength(500);

        entity.Property(e => e.PhotoUrl)
            .HasMaxLength(2048);

        //relations
        entity.HasOne(cp => cp.User)
            .WithOne(u => u.ConsumerProfile)
            .HasForeignKey<ConsumerProfile>(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt == null);
    }
}
