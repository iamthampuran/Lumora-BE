using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Common.Queries.GetTags;

public class GetTagsQueryHandler(ILogger<GetTagsQueryHandler> logger, ITagRepository tagRepository)
{
    public async Task<Result<IEnumerable<GetTagsQueryResponse>>> Handle(GetTagsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetTagsQuery));
        var response = await tagRepository.GetTagsAsync(query.SearchText, cancellationToken);
        return Result.Success(response);
    }
}
