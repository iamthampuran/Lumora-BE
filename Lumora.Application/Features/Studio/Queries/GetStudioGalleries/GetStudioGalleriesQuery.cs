using Lumora.Application.Helpers;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Studio.Queries.GetStudioGalleries;

public record GetStudioGalleriesQuery(GalleryStatus? GalleryStatus, PaginationOptions PaginationOptions, string? SearchText);

