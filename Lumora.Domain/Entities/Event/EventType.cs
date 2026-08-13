using Lumora.Domain.Entities.Common;

namespace Lumora.Domain.Entities.Event;

public class EventType : BaseEntity
{
    public string Name { get; set; } = null!;
    public bool IsPredefined { get; set; } = true;  // Always true for this table

    public virtual IEnumerable<Event> Events { get; set; } = null!;

    public EventType(string name, bool isPredefined = true)
    {
        Name = name;
        IsPredefined = isPredefined;
    }
}
