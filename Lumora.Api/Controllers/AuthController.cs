using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Auth.Commands.CreateConsumer;
using Lumora.Application.Features.Auth.Commands.CreateStudio;
using Lumora.Application.Features.Auth.Commands.SignupAccount;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wolverine;

namespace Lumora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMessageBus messageBus) : ControllerBase
    {
        [HttpPost("create/user")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] SignupAccountCommand command, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<Guid>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPost("create/studio/{userId}")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<Guid>> CreateStudio([FromRoute] Guid userId, [FromBody] CreateStudioCommand command, CancellationToken cancellationToken)
        {
            if (userId != command.UserId)
                return BadRequest();
            var result = await messageBus.InvokeAsync<Result<Guid>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPost("create/consumer/{userId}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Guid>> CreateConsumer([FromRoute] Guid userId, [FromForm] CreateConsumerDto request, IFormFile? formFile, CancellationToken cancellationToken)
        {
            if (userId != request.UserId)
                return BadRequest("UserId found in route and form are different.");

            var command = new CreateConsumerCommand
            {
                UserId = request.UserId,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Bio = request.Bio,
                FileDetails = formFile is not null ? new FileDetails(formFile.OpenReadStream(), formFile.ContentType) : null
            };

            var result = await messageBus.InvokeAsync<Result<Guid>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        public class CreateConsumerRequest
        {
            public Guid UserId { get; init; }
            public string FullName { get; init; } = null!;
            public string PhoneNumber { get; init; } = null!;
            public string? Bio { get; init; }
            public IFormFile? FormFile { get; init; }
        }
    }
}
