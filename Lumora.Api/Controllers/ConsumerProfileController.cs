using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Consumer.Commands.AddProfilePicture;
using Lumora.Application.Features.Consumer.Queries.GetDashboardTable;
using Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;
using Lumora.Application.Helpers;
using Lumora.Domain.Enums;
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

    [HttpGet("{id}/dashboard/events")]
    [ProducesResponseType(typeof(GetDashboardTableQueryResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GetDashboardTableQueryResponse>> GetConsumerDashboard([FromRoute] Guid id, [FromQuery] EventStatus eventStatus,
        [FromQuery] PaginationOptions? paginationOptions,
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardTableQuery(id, eventStatus, paginationOptions ?? new PaginationOptions());
        var result = await messageBus.InvokeAsync<Result<GetDashboardTableQueryResponse>>(query, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("{id}/dashboard/inquiries")]
    [ProducesResponseType(typeof(IEnumerable<GetInquiryWidgetResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<GetInquiryWidgetResponse>>> GetInquiryWidgetDetails([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<IEnumerable<GetInquiryWidgetResponse>>>(new GetInquiryWidgetQuery(id), cancellationToken);
        return result.ToActionResult(this);
    }

}
