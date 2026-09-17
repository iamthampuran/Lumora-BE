using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Auth.Commands.CreateConsumer;
using Lumora.Application.Features.Auth.Commands.CreateStudio;
using Lumora.Application.Features.Auth.Commands.Initiate2FASetup;
using Lumora.Application.Features.Auth.Commands.LogoutUser;
using Lumora.Application.Features.Auth.Commands.SignInUser;
using Lumora.Application.Features.Auth.Commands.SignupAccount;
using Lumora.Application.Features.Auth.Commands.Verify2FALogin;
using Lumora.Application.Features.Auth.Commands.VerifyAndEnable2FA;
using Lumora.Application.Features.Auth.Queries.GetConsumerData;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("signin")]
        [ProducesResponseType(typeof(SignInUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<SignInUserResponse>> SignIn([FromBody] SignInUserCommand command, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<SignInUserResponse>> (command, cancellationToken);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpPost("2fa/initiate")]
        [ProducesResponseType(typeof(Initiate2FASetupResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Initiate2FASetupResponse>> Initiate2FASetup([FromBody] Initiate2FASetupCommand command, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<Initiate2FASetupResponse>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpPost("2fa/verify-and-enable")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<string>>> VerifyAndEnable2FA([FromBody] VerifyAndEnable2FACommand command, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<List<string>>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpDelete("logout/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<int>> Logout([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<int>>(new LogoutUserCommand(userId), cancellationToken);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpGet()]
        [ProducesResponseType(typeof(GetConsumerDataResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<GetConsumerDataResponse>> GetConsumerData(CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<GetConsumerDataResponse>>(new GetConsumerDataQuery(), cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPost("2fa/verify-login")]
        [ProducesResponseType(typeof(SignInUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<SignInUserResponse>> VerifyLogin(Verify2FALoginCommand command, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<SignInUserResponse>>(command, cancellationToken);
            return result.ToActionResult(this);
        }


    }
}
