using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Entities.Studio.ValueObjects;

namespace Lumora.Domain.Entities.Studio;

public class Employee : BaseEntity
{
    public Guid StudioId { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public EmployeeRole EmployeeRole { get; set; } = null!;

    //navigation property
    public virtual StudioProfile Studio { get; set; } = null!;
}
