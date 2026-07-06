using Lumora.Domain.Entities.Common;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities.Studio.ValueObjects;

public class EmployeeRole
{
    public string Type { get; init; }      // "Predefined" or "Custom"
    public string Value { get; init; }     // "MainPhotographer" or custom

    public EmployeeRole(string type, string value)
    {
        Type = type;
        Value = value;
    }

    // Parameterless constructor for EF Core
    public EmployeeRole() { }
}
