using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Consumer.Commands.AddProfilePicture;
using Lumora.Application.Features.Consumer.Commands.CreateEvent;
using Lumora.Application.Features.Consumer.Queries.GetDashboardTable;
using Lumora.Application.Features.Consumer.Queries.GetEventById;
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
        [FromQuery] PaginationOptions? paginationOptions, [FromQuery] string? searchText,
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardTableQuery(id, eventStatus, paginationOptions ?? new PaginationOptions(), searchText);
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

    [HttpPost("{id}/create/event")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Guid>> CreateEvent([FromRoute] Guid id, [FromBody] CreateEventCommand command, CancellationToken cancellationToken)
    {
        if (command.ConsumerId != id)
        {
            return BadRequest("Consumer ID in the route does not match the Consumer ID in the request body.");
        }
        var result = await messageBus.InvokeAsync<Result<Guid>>(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("event/{id}")]
    [ProducesResponseType(typeof(GetEventByIdQueryResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<GetEventByIdQueryResponse>> GetEventDetailsById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<GetEventByIdQueryResponse>>(new GetEventByIdQuery(id), cancellationToken);
        return result.ToActionResult(this);
    }

    //[HttpGet("events/{id}")]
    
}
