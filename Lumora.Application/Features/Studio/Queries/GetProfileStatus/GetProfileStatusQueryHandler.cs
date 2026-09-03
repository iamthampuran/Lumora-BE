using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Queries.GetProfileStatus;

public class GetProfileStatusQueryHandler(ILogger<GetProfileStatusQueryHandler> logger, IStudioRepository studioRepository)
{
    public async Task<Result<ProfileCompletionResult>> Handle(GetProfileStatusQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetProfileStatusQuery));
        var studio = await studioRepository.GetFirstAsync(s => s.Id == query.StudioId, null, [s => s.Employees, s => s.Tags, s => s.PortfolioImages], true, cancellationToken);
        if (studio == null)
        {
            return Result.NotFound("Studio details not found in db :(");
        }

        return Result.Success(studio.GetProfileCompletion());
    }
}
