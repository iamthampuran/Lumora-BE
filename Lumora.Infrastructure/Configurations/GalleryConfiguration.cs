using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class GalleryConfiguration
{
    public static void ConfigureGallery(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Gallery>();

        entity.HasKey(g => g.Id);

        //validations
        entity.Property(g => g.InquiryId)
            .IsRequired();

        entity.Property(g => g.GalleryName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(g => g.Description)
            .HasMaxLength(500);

        entity.Property(g => g.GalleryStatus)
            .IsRequired()
            .HasDefaultValue(GalleryStatus.Draft)
            .HasConversion<string>();

        entity.Property(g => g.FolderLink)
            .IsRequired();

        entity.Property(g => g.UploadedAt)
            .IsRequired(false);

        entity.Property(g => g.ApprovedAt)
            .IsRequired(false);

        //relationship



        entity.HasIndex(g => g.InquiryId)
            .IsUnique();

        entity.HasIndex(g => g.GalleryStatus);
        entity.HasIndex(g => g.ExternalProvider);
        entity.HasIndex(g => g.ApprovedAt);

        entity.HasQueryFilter(g => g.IsActive && g.DeletedAt == null);
    }
}
