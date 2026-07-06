using Lumora.Domain.Entities.Reviews;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class ReviewConfiguration
{
    public static void ConfigureReview(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Review>();

        entity.Property(r => r.InquiryId)
            .IsRequired();

        entity.Property(r => r.ConsumerId)
            .IsRequired();

        entity.Property(r => r.StudioId)
            .IsRequired();

        entity.Property(r => r.Rating)
            .IsRequired()
            .HasPrecision(2,1);

        entity.Property(r => r.Title)
            .HasMaxLength(200);

        entity.Property(r => r.Comment)
            .HasMaxLength(1000);

        //relationships
        entity.HasOne(r => r.Studio)
            .WithMany()
            .HasForeignKey(r => r.StudioId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(r => r.Consumer)
            .WithMany()
            .HasForeignKey(r => r.ConsumerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(r => r.Inquiry)
            .WithMany(i => i.Reviews)
            .HasForeignKey(r => r.InquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(r => r.InquiryId)
            .IsUnique();

        entity.HasIndex(r => r.StudioId);
        entity.HasIndex(r => r.ConsumerId);
        entity.HasIndex(r => r.Rating);

        entity.HasQueryFilter(r => r.IsActive && r.DeletedAt == null);

    }
}
