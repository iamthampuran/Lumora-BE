using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Consumer.Commands.AddProfilePicture;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wolverine;

namespace Lumora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConsumerProfileController(IMessageBus messageBus) : ControllerBase
{
    [HttpPatch("{id}/update/profile/picture")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<string>> UpdateUserProfile([FromRoute] Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        var command = new AddProfilePictureCommand(file.OpenReadStream(), id, file.ContentType);
        var result = await messageBus.InvokeAsync<Result<string>>(command, cancellationToken);
        return result.ToActionResult(this);
    }

}
