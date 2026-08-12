using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Queries.GetDashboardTable;

public class GetDashboardTableQueryHandler(ILogger<GetDashboardTableQueryHandler> logger, IEventRepository eventRepository)
{
    public async Task<Result<GetDashboardTableQueryResponse>> Handle(GetDashboardTableQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetDashboardTableQuery));

        var response = await eventRepository.GetConsumerEventsAsync(request.Id, request.Status, request.PaginationOptions, cancellationToken);
        return Result.Success(response);
    }
}
