using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Identity;

namespace Lumora.Domain.Entities.Studio;

public class StudioTag : BaseEntity
{
    public Guid StudioProfile { get; set; }
    public Guid TagId { get; set; }

    public virtual StudioProfile Studio { get; set; } = null!;
    public virtual Tag.Tag Tag { get; set; } = null!;

}
