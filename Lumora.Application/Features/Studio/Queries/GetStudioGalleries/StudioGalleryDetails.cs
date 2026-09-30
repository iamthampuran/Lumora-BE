using Lumora.Application.Helpers;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Studio.Queries.GetStudioGalleries;

public record StudioGalleryDetails(Guid Id, string GalleryName, GalleryStatus GalleryStatus, string GalleryCoverUrl, DateOnly CreatedDate);

public record RecentEventDetails(Guid Id, string Title, DateOnly EventDate, string Location, string Consumer, EventStatus Status);

public record GetStudioGalleriesResponse(RecentEventDetails? EventDetails, PaginatedResponse<StudioGalleryDetails> GalleryDetails);