using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Consumer.Commands.InitiatePayment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Lumora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IMessageBus messageBus) : ControllerBase
    {
        [Authorize]
        [HttpPost("generate-qr")]
        public async Task<ActionResult<InitiatePaymentResponse>> InitiatePayment([FromBody] InitiatePaymentCommand command, CancellationToken cancellationToken)
        {
            var result =  await messageBus.InvokeAsync<Result<InitiatePaymentResponse>>(command, cancellationToken);
            return result.ToActionResult(this);
        }
    }
}
