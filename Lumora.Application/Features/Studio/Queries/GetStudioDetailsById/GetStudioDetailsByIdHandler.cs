using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Queries.GetStudioDetailsById;

public class GetStudioDetailsByIdHandler(ILogger<GetStudioDetailsByIdHandler> logger, IStudioRepository studioRepository)
{
    public async Task<Result<GetStudioDetailsByIdResponse>> Handle(GetStudioDetailsByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetStudioDetailsByIdQuery));
        var studioDetails = await studioRepository.GetStudioDashboardByIdAsync(query.Id, cancellationToken);
        if (studioDetails == null)
        {
            return Result.NotFound("Studio with the id was not found");
        }
        return Result.Success(studioDetails);
    }
}
