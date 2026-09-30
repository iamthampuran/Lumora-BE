using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Features.Studio.Queries.GetStudioGalleries;
using Lumora.Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wolverine;

namespace Lumora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GalleryController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("studio")]
    [ProducesResponseType(typeof(GetStudioGalleriesResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<GetStudioGalleriesResponse>> GetStudioGalleries([FromQuery] GetStudioGalleriesQuery query, CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<GetStudioGalleriesResponse>>(query, cancellationToken);
        return result.ToActionResult(this);
    }
}
