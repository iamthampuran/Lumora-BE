using Ardalis.Result;
using Ardalis.Result.AspNetCore;
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

    }
}
