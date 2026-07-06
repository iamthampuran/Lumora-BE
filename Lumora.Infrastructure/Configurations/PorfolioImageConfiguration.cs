using Lumora.Domain.Entities.Studio;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class PorfolioImageConfiguration
{
    public static void ConfigurePortoflioImage(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<PortfolioImage>();

        entity.HasKey(p => p.Id);

        entity.Property(p => p.StudioId)
            .IsRequired();

        entity.Property(p => p.ImageUrl)
            .IsRequired()
            .HasMaxLength(2000);

        entity.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(p => p.DisplayOrder)
            .IsRequired();

        entity.HasOne(p => p.StudioProfile)
            .WithMany(s => s.PortfolioImages)
            .HasForeignKey(p => p.StudioId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(p => p.StudioId);
        entity.HasIndex(p => new { p.StudioId, p.DisplayOrder });

        entity.HasQueryFilter(p => p.IsActive && p.DeletedAt == null);
    }
}
