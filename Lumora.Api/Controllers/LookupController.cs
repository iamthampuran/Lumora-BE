using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Common.Queries.GetEventCategories;
using Lumora.Application.Features.Common.Queries.GetTags;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wolverine;

namespace Lumora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LookupController (IMessageBus messageBus) : ControllerBase
{
    [ProducesResponseType(typeof(IEnumerable<GetEventTypesQueryResponse>), (int)HttpStatusCode.OK)]
    [HttpGet("event-types")]
    public async Task<ActionResult<IEnumerable<GetEventTypesQueryResponse>>> GetEventTypes([FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<IEnumerable<GetEventTypesQueryResponse>>>(new GetEventTypesQuery(searchText), cancellationToken);
        return result.ToActionResult(this);
    }

    [ProducesResponseType(typeof(IEnumerable<GetTagsQueryResponse>), (int)HttpStatusCode.OK)]
    [HttpGet("tags")]
    public async Task<ActionResult<IEnumerable<GetTagsQueryResponse>>> GetTags([FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<IEnumerable<GetTagsQueryResponse>>>(new GetTagsQuery(searchText), cancellationToken);
        return result.ToActionResult(this);
    }
}
