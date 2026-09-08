using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Identity;

namespace Lumora.Domain.Entities.Studio;

public class PortfolioImage : BaseEntity
{
    public Guid StudioId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? Title { get; set; }
    public int DisplayOrder { get; set; }

    public virtual StudioProfile StudioProfile { get; set; } = null!;
}
