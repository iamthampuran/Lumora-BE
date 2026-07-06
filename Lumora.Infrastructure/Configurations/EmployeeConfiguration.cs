using Lumora.Domain.Entities.Studio;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Lumora.Infrastructure.Configurations;

public static class EmployeeConfiguration
{
    public static void ConfigureEmployee(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Employee>();

        entity.HasKey(e => e.Id);

        entity.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(20);

        entity.OwnsOne(e => e.EmployeeRole, ero =>
        {
            ero.Property(er => er.Type).HasColumnName("Employee Role Type");
            ero.Property(er => er.Value).HasColumnName("Employee Role Value");
        });

        entity.HasOne(e => e.Studio)
            .WithMany(s => s.Employees)
            .HasForeignKey(e => e.StudioId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(e => e.Email);
        entity.HasIndex(e => new { e.Email, e.Phone }).IsUnique();

        entity.HasQueryFilter(e => e.IsActive && e.DeletedAt != null);
    }
}
