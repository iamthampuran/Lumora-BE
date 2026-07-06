using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities.Payments;

public class Payment : BaseEntity
{
    public Guid InquiryId { get; set; } 
    public Guid EventId { get; set; }
    public Guid StudioId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public PaymentStatus Status { get; set; }
    public string RazorPayOrderId { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public DateTime InitiatedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? RazorePaySignature { get; set; }

    //navigation property
    public virtual Inquiry Inquiry { get; set; } = null!;
    public virtual StudioProfile Studio { get; set; } = null!;
    public virtual Event.Event Event { get; set; } = null!;

}
