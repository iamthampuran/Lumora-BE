using Lumora.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Configurations;

public static class PaymentConfiguration
{
    public static void ConfigurePayment(this ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Payment>();

        entity.HasKey(p => p.Id);

        entity.Property(p => p.InquiryId)
            .IsRequired();

        entity.Property(p => p.EventId)
            .IsRequired();

        entity.Property(p => p.StudioId)
            .IsRequired();

        entity.Property(p => p.Amount)
            .IsRequired();

        entity.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("INR");

        entity.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        entity.Property(p => p.RazorPayOrderId)
            .IsRequired()
            .HasMaxLength(50);

        entity.Property(p => p.InitiatedAt)
            .IsRequired();

        entity.Property(p => p.TransactionId)
            .HasMaxLength(50);

        entity.Property(p => p.RazorePaySignature)
            .HasMaxLength(64);

        entity.Property(p => p.FailureReason)
            .HasMaxLength(500);

        //relationships

        //entity.HasOne(p => p.Inquiry)
        //    .WithOne()
        //    .HasForeignKey<Payment>(p => p.InquiryId)
        //    .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(p => p.Studio)
            .WithMany()
            .HasForeignKey(p => p.StudioId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(p => p.Event)
            .WithMany()
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        //unique constraint
        entity.HasIndex(p => p.RazorPayOrderId)
            .IsUnique();


        //indexing for queries
        entity.HasIndex(p => p.Status);
        entity.HasIndex(p => p.StudioId);
        entity.HasIndex(p => p.EventId);

        entity.HasQueryFilter(p => p.IsActive && p.DeletedAt == null);

    }
}
