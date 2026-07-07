using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Studio.Queries.GetStudioById;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wolverine;
namespace Lumora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudioController(IMessageBus messageBus) : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetStudioByIdResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<GetStudioByIdResponse>> GetStudioDetailsById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<GetStudioByIdResponse>>(new GetStudioByIdQuery(id), cancellationToken);
            return result.ToActionResult(this);
        }
    }
}
