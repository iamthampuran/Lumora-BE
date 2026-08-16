using Lumora.Domain.Entities.Tag;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class TagConfiguration
{
    public static void ConfigureTag(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Tag>();

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasIndex(e => e.Name)
            .IsUnique();

        // Studio-Tag: One-to-many via explicit StudioTag association
        entity.HasMany(t => t.StudioTags)
            .WithOne(st => st.Tag)
            .HasForeignKey(st => st.TagId);

        // Event-Tag: One-to-many via explicit EventTag association
        entity.HasMany(t => t.EventTags)
            .WithOne(et => et.Tag)
            .HasForeignKey(et => et.TagId);

        // Soft delete support
        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt == null);


        entity.HasData(
        new
        {
            Id = Guid.Parse("df699af6-7d5e-4c0e-84df-70ba1c2c736b"),
            Name = "cinematic",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("accda9f2-4ca7-4102-8cc1-40348ea42a52"),
            Name = "documentary",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("f1d92970-e307-4997-9b7a-27527afd2173"),
            Name = "moody",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("d4702155-2ae0-445a-9fb3-cf6b04966bba"),
            Name = "candid",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("68f3bb15-c4bb-41a8-912c-49563b2386d2"),
            Name = "traditional",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("d7bc3b61-4fb9-4802-a83f-f7c5c91c521f"),
            Name = "editorial",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("a13145e5-bd2d-4433-b1f4-cd17e4b3d093"),
            Name = "film",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("c15e9002-b7eb-48a3-b197-5c7bfb22c806"),
            Name = "drone",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("51a4aebc-2cbe-4ce1-b95f-05a88ff163ab"),
            Name = "corporate",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        },
        new
        {
            Id = Guid.Parse("981479f3-3a6a-4b59-ac3e-3199edfd6a93"),
            Name = "wedding",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            DeletedAt = (DateTime?)null,
            CreatedBy = "system",
            ModifiedBy = "system"
        });

    }
}
