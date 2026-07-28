using Lumora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class RefreshTokenConfiguration
{
    public static void ConfigureRefreshToken(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RefreshToken>();

        entity.HasKey(x => x.Id);

        entity.Property(x => x.UserId)
            .IsRequired();
        entity.Property(x => x.Token)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.Property(x => x.ExpiresAt)
            .IsRequired();

        entity.HasIndex(x => x.Token)
            .IsUnique();

        entity.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasQueryFilter(rt => rt.User.DeletedAt == null && rt.User.IsActive);
    }
}
