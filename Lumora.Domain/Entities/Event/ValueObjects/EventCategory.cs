using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities.Event.ValueObjects;

public class EventCategory
{
    public string Type { get; init; }
    public string Value { get; init; } = null!;

    public EventCategory(string type, string value)
    {
        Type = type;
        Value = value;
    }
    public EventCategory()
    {
        
    }

}
