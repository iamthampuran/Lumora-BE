using Ardalis.Result;
using Lumora.Application.Configuration;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lumora.Application.Features.Studio.Queries.GetStudioById;

public class GetStudioByIdQueryHandler(ILogger<GetStudioByIdQueryHandler> logger, IStudioRepository studioRepository)
{
    public async Task<Result<GetStudioByIdResponse>> Handle(GetStudioByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetStudioByIdQueryHandler));
        ArgumentNullException.ThrowIfNull(nameof(query));
        var studio = await studioRepository.GetStudioDetailsByIdAsync(query.Id, cancellationToken);
        return studio != null ? Result.Success(studio) : Result.NotFound();
    }
}
