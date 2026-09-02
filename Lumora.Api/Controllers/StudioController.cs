using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Studio.Commands.UpdateCover;
using Lumora.Application.Features.Studio.Commands.UpdateLogo;
using Lumora.Application.Features.Studio.Queries.GetProfileStatus;
using Lumora.Application.Features.Studio.Queries.GetStudioById;
using Lumora.Domain.Entities.Identity;
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

        [HttpPatch("{id}/update-logo")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<string>> UpdateStudioLogo([FromRoute] Guid id, IFormFile formFile, CancellationToken cancellationToken)
        {
            var command = new UpdateLogoCommand(formFile.OpenReadStream(), id, formFile.ContentType);
            var result = await messageBus.InvokeAsync<Result<string>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpGet("{id}/profile-completion")]
        [ProducesResponseType(typeof(ProfileCompletionResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProfileCompletionResult>> GetProfileCompletionDetails([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<ProfileCompletionResult>>(new GetProfileStatusQuery(id), cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPatch("{id}/update-cover")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<string>> UpdateCover([FromRoute] Guid id, IFormFile formFile, CancellationToken cancellationToken)
        {
            var command = new UpdateCoverCommand(formFile.OpenReadStream(), id, formFile.ContentType);
            var result = await messageBus.InvokeAsync<Result<string>>(command, cancellationToken);
            return result.ToActionResult(this);
        }
    }
}
