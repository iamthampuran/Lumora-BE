using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class InquiryConfiguration
{
    public static void ConfigureInquiry(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Inquiry>();

        entity.HasKey(x => x.Id);

        entity.Property(e => e.Message)
            .HasMaxLength(1000);

        entity.Property(e => e.RejectionStatus)
            .HasMaxLength(1000);

        entity.Property(e => e.QuotedAmount)
            .HasPrecision(8, 2);

        entity.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>();

        entity.HasOne(i => i.Studio)
            .WithMany()
            .HasForeignKey(i => i.StudioId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(i => i.Event)
            .WithMany(e => e.Inquiries)
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Consumer)
            .WithMany()
            .HasForeignKey(e => e.ConsumerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(i => i.Gallery)
            .WithOne(g => g.Inquiry)
            .HasForeignKey<Gallery>(g => g.InquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(i => i.Payment)
            .WithOne(p => p.Inquiry)
            .HasForeignKey<Payment>(p => p.InquiryId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(i => i.Reviews)
            .WithOne(r => r.Inquiry)
            .HasForeignKey(r => r.InquiryId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(i => new { i.StudioId, i.EventId, i.ConsumerId })
            .IsUnique();

        entity.HasIndex(i => i.StudioId);
        entity.HasIndex(i => i.EventId);
        entity.HasIndex(i => i.ConsumerId);
        entity.HasIndex(i => i.Status);

        entity.ToTable(t => t.HasCheckConstraint(
            "CK_Inquiry_QuotedAmount",
            "\"QuotedAmount\" >= 0 AND \"QuotedAmount\" <= 100000"
        ));

        entity.HasQueryFilter(i => i.IsActive && i.DeletedAt == null);
    }
}
