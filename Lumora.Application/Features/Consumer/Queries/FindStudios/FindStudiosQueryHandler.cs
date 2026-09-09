using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Helpers;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Queries.FindStudios;

public class FindStudiosQueryHandler(IStudioRepository studioRepository, ILogger<FindStudiosQueryHandler> logger, IEventRepository eventRepository)
{
    public async Task<Result<PaginatedResponse<FindStudiosQueryResponse>>> Handle(FindStudiosQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(FindStudiosQuery));
        var eventData = await eventRepository.GetFirstAsync(e => e.Id == query.EventId, null, [e => e.EventTags], true, cancellationToken);
        if (eventData == null)
        {
            return Result.NotFound("Event with the data not found");
        }

        var studioData = await studioRepository.GetRecommendedStudiosAsync(eventData, query.StudioFilter, query.SortOption, query.PaginationOptions, cancellationToken);
        return Result.Success(studioData);
    }
}
