using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Entities.Payments;
using Lumora.Domain.Entities.Reviews;
using Lumora.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lumora.Domain.Entities.Event;

public class Inquiry : BaseEntity
{
    public Guid EventId { get; private set; }
    public Guid StudioId { get; private set; }
    public Guid ConsumerId { get; private set; }
    public string? Message { get; set; }
    public InquiryStatus Status { get; set; } = InquiryStatus.Submitted;
    public string? RejectionStatus {  get; set; }

    [Range(typeof(decimal), "0", "100000")]
    public decimal? QuotedAmount { get; set; }

    //navigation property
    public virtual Event Event { get; set; } = null!;
    public virtual StudioProfile Studio { get; set; } = null!;
    public virtual ConsumerProfile Consumer { get; set; } = null!;
    public virtual Gallery? Gallery { get; set; } 
    public virtual Payment? Payment { get; set; }
    public virtual ICollection<Review> Reviews { get; set; } = [];
    

}
