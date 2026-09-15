using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Consumer.Commands.UpdateEvent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wolverine;

namespace Lumora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController(IMessageBus messageBus) : ControllerBase
    {
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Guid>> UpdateEvent([FromRoute] Guid id, [FromBody] UpdateEventCommand command, CancellationToken cancellationToken)
        {
            if (command.Id != id)
            {
                return BadRequest("Id in the path doesn't match the id in the body");
            }
            var result = await messageBus.InvokeAsync<Result<Guid>>(command, cancellationToken);
            return result.ToActionResult(this);
        }
    }
}
