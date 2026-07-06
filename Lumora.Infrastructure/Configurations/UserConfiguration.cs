using Lumora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class UserConfiguration
{
    public static void ConfigureUser(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<User>();

        //key
        entity.HasKey(x => x.Id);

        //properties
        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Role)
            .IsRequired()
            .HasConversion<string>();

        entity.Property(e => e.TwoFactorSecret)
            .IsRequired(false)
            .HasMaxLength(255);

        entity.OwnsOne(e => e.SuspensionInfo, si =>
        {
            si.ToJson();
        });

        //index
        entity.HasIndex(e => e.Email)
            .IsUnique();

        entity.HasIndex(e => e.Role);

        //navigation
        entity.HasOne(e => e.ConsumerProfile)
            .WithOne()
            .HasForeignKey<ConsumerProfile>(cp => cp.Id)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.StudioProfile)
            .WithOne()
            .HasForeignKey<StudioProfile>(sp => sp.Id)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt == null);
    }
}
