using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Common.Queries.GetEventCategories;

public class GetEventTypesQueryHandler(IEventTypeRepository eventTypeRepository, ILogger<GetEventTypesQueryHandler> logger)
{
    public async Task<Result<IEnumerable<GetEventTypesQueryResponse>>> Handle(GetEventTypesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetEventTypesQueryHandler));
        var eventTypes = await eventTypeRepository.GetEventTypes(request.SearchText, cancellationToken);
        return Result.Success(eventTypes);
    }
}
