using Lumora.Application.Features.Studio.Queries.GetStudioGalleries;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;
namespace Lumora.Application.Contracts.Persistence;

public interface IGalleryRepository : IGenericRepository<Gallery>
{
    Task<PaginatedResponse<StudioGalleryDetails>> GetStudioGalleries(Guid studioId, GalleryStatus? status, PaginationOptions paginationOptions, string? searchText, CancellationToken cancellationToken);
}
