using Lumora.Domain.Entities.Event;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class EventTypeConfiguration
{
    public static void ConfigureEventType(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EventType>();

        entity.HasKey(x => x.Id);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.HasIndex(e => e.Name).IsUnique();

        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt == null);

        // Seed predefined event types with static UTC values
        entity.HasData(
         new { Id = new Guid("672fce47-95c8-42d1-b65d-ac33bfdf2b02"), Name = "Wedding", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 13, 9, 48, 24, 208, DateTimeKind.Utc).AddTicks(7728), ModifiedAt = new DateTime(2026, 8, 13, 15, 18, 24, 208, DateTimeKind.Utc).AddTicks(8300), DeletedAt = (DateTime?)null },
         new { Id = new Guid("9e3adba4-24a2-440d-9d3e-902e92e390f9"), Name = "Birthday", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(529), ModifiedAt = new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(537), DeletedAt = (DateTime?)null },
         new { Id = new Guid("f355707c-0563-4d8a-bfa2-79129e91ef4a"), Name = "Corporate", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(549), ModifiedAt = new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(551), DeletedAt = (DateTime?)null },
         new { Id = new Guid("482fbf1c-b62f-4704-bfeb-8da35b8a6d42"), Name = "Engagement", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(552), ModifiedAt = new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(553), DeletedAt = (DateTime?)null },
         new { Id = new Guid("4aab92d6-f1fa-4fa7-8c3b-23f05fb29015"), Name = "Anniversary", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(559), ModifiedAt = new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(560), DeletedAt = (DateTime?)null },
         new { Id = new Guid("208f807f-b0de-42e8-8518-2e1d103f4318"), Name = "Pre-Wedding", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 13, 9, 48, 24, 211, DateTimeKind.Utc).AddTicks(561), ModifiedAt = new DateTime(2026, 8, 13, 15, 18, 24, 211, DateTimeKind.Utc).AddTicks(562), DeletedAt = (DateTime?)null },

         new { Id = new Guid("d6f58d6f-43f5-4be8-9a34-2e45f9a1a101"), Name = "Reception", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), ModifiedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), DeletedAt = (DateTime?)null },
         new { Id = new Guid("57f0de02-5aa8-43bd-a0f8-04f4f49f2102"), Name = "Haldi", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), ModifiedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), DeletedAt = (DateTime?)null },
         new { Id = new Guid("f9e17d4f-5f9b-4604-af70-65a3c30c6b03"), Name = "Baby Shower", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), ModifiedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), DeletedAt = (DateTime?)null },
         new { Id = new Guid("9b4c75db-4fd8-4f64-8d26-7b3ae16bb004"), Name = "Maternity Shoot", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), ModifiedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), DeletedAt = (DateTime?)null },
         new { Id = new Guid("79c8f6c9-f717-4d2e-9269-b66712a94805"), Name = "Housewarming", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), ModifiedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), DeletedAt = (DateTime?)null },
         new { Id = new Guid("2d25cce8-8923-4c57-8f83-70de9b04c006"), Name = "Graduation", IsPredefined = true, CreatedBy = "System", ModifiedBy = "System", IsActive = true, CreatedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), ModifiedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc), DeletedAt = (DateTime?)null }
     );
    }
}
