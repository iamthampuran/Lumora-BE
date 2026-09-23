using Lumora.Domain.Entities.Event;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class InquiryEmployeeConfiguration
{
    public static void ConfigureInquiryEmployee(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<InquiryEmployee>();

        // Composite key prevents duplicate employee assignments per inquiry
        entity.HasKey(ie => new { ie.InquiryId, ie.EmployeeId });

        // Inquiry -> InquiryEmployee (one-to-many)
        entity.HasOne(ie => ie.Inquiry)
            .WithMany(i => i.InquiryEmployees)
            .HasForeignKey(ie => ie.InquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Employee -> InquiryEmployee (one-to-many)
        entity.HasOne(ie => ie.Employee)
            .WithMany(e => e.Inquiries)
            .HasForeignKey(ie => ie.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Performance indexes
        entity.HasIndex(ie => ie.InquiryId);
        entity.HasIndex(ie => ie.EmployeeId);

        // Soft delete support
        entity.HasQueryFilter(ie => ie.IsActive && ie.DeletedAt == null);
    }
}
