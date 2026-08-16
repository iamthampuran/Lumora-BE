using Lumora.Domain.Entities.Common;

namespace Lumora.Domain.Entities.Event;

public class EventTag : BaseEntity
{
    public Guid EventId {  get; set; }
    public Guid TagId {  get; set; }
    public virtual Event Event { get; set; } = null!;
    public virtual Domain.Entities.Tag.Tag Tag { get; set; } = null!;
}
