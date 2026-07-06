using Lumora.Domain.Entities.Common;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities.Event;

public class Gallery : BaseEntity
{
    public Guid InquiryId { get; set; }
    public string GalleryName { get; set; } = null!;
    public string? Description { get; set; }
    public GalleryStatus GalleryStatus { get; set; }
    public ExternalProvider ExternalProvider { get; set; }
    public string FolderLink { get; private set; } = string.Empty;
    public DateTime? UploadedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }

    //navigation property
    public virtual Inquiry Inquiry { get; set; } = null!;
}
