using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Queries.GetStudioGalleries;

public class GetStudioGalleriesHandler(ICurrentUserService currentUserService, ILogger<GetStudioGalleriesHandler> logger, IGalleryRepository galleryRepository, IInquiryRepository inquiryRepository)
{
    public async Task<Result<GetStudioGalleriesResponse>> Handle(GetStudioGalleriesQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetStudioGalleriesQuery));
        var userDetails = currentUserService.GetCurrentUserDetails();

        if (userDetails == null)
            return Result.Unauthorized("User not authenticated");

        if (userDetails.StudioId == null)
            return Result.Forbidden("User does not have the permission for viewinig this");

        var galleryDetails = await galleryRepository.GetStudioGalleries(userDetails.StudioId.Value, query.GalleryStatus, query.PaginationOptions, query.SearchText, cancellationToken);
         
        var latestInquiry = await inquiryRepository.GetFirstAsync(i => i.Event.Status == Domain.Enums.EventStatus.InProgress && i.StudioId == userDetails.StudioId.Value,
            i => i.OrderByDescending(i => i.ModifiedAt), 
            [i => i.Event, i => i.Event.Consumer],
            true,
            cancellationToken);

        if (latestInquiry is not null)
        {
            var requiredData = new RecentEventDetails(latestInquiry.EventId,
                latestInquiry.Event.Title,
                latestInquiry.Event.EventDate,
                latestInquiry.Event.Location.LocationName,
                latestInquiry.Event.Consumer.FullName,
                latestInquiry.Event.Status);
            return Result.Success(new GetStudioGalleriesResponse(requiredData, galleryDetails));
        }

        return Result.Success(new GetStudioGalleriesResponse(null, galleryDetails));
    }
}
