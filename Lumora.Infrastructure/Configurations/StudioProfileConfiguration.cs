using Lumora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class StudioProfileConfiguration
{
    public static void ConfigureStudioProfile(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<StudioProfile>();

        entity.HasKey(x => x.Id);

        entity.Property(e => e.StudioName)
            .IsRequired()
            .HasMaxLength(300);

        entity.Property(e => e.Phone)
            .HasMaxLength(20);

        entity.Property(s => s.MinPrice)
            .IsRequired()
            .HasPrecision(18, 2); // 18 total digits, 2 decimal places

        entity.Property(s => s.MaxPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        entity.Property(e => e.UserId)
            .IsRequired();


        entity.OwnsOne(e => e.Location, lo => lo.ToJson());
        entity.OwnsOne(e => e.ServiceRadius, sr => sr.ToJson());

        entity.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<StudioProfile>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.Employees)
            .WithOne(e => e.Studio)
            .HasForeignKey(e => e.StudioId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.Tags)
            .WithMany(t => t.Studios)
            .UsingEntity("StudioTags");

        entity.HasIndex(e => e.StudioName);

        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt == null);


    }
}

