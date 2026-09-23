using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Studio;

namespace Lumora.Domain.Entities.Event;

public class InquiryEmployee : BaseEntity
{
    public Guid InquiryId { get; set; }
    public Guid EmployeeId { get; set; }

    public virtual Inquiry Inquiry { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
}
