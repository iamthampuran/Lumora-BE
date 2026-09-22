using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Features.Studio.Commands.AcceptInquiry;
using Lumora.Application.Features.Studio.Commands.AddEmployees;
using Lumora.Application.Features.Studio.Commands.AddPortfolioImage;
using Lumora.Application.Features.Studio.Commands.AddTagsToStudio;
using Lumora.Application.Features.Studio.Commands.UpdateCover;
using Lumora.Application.Features.Studio.Commands.UpdateLogo;
using Lumora.Application.Features.Studio.Commands.UpdatePortfolioImage;
using Lumora.Application.Features.Studio.Queries.GetInquiries;
using Lumora.Application.Features.Studio.Queries.GetInquiryDetails;
using Lumora.Application.Features.Studio.Queries.GetProfileStatus;
using Lumora.Application.Features.Studio.Queries.GetStudioDetailsById;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Wolverine;
using Wolverine.Runtime;
namespace Lumora.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudioController(IMessageBus messageBus) : ControllerBase
    {

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

        [HttpPost("{id}/add-portfolio-images")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Guid>> AddPortfolioImages([FromRoute] Guid id, IFormFile file, [FromForm] string? title, [FromForm] int order, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<Guid>>(new AddPortfolioImageCommand(id, file.OpenReadStream(), order, title, file.ContentType), cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPut("update-portfolio-image/{id}")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Guid>> UpdatePortfolioImage([FromRoute] Guid id, IFormFile? file, [FromForm] string? title, [FromForm] int? order,
            CancellationToken cancellationToken, [FromForm] bool isDeleted = false)
        {
            var result = await messageBus.InvokeAsync<Result<Guid>>(new UpdatePortfolioImageCommand(title, file?.OpenReadStream(), file?.ContentType, order, id, isDeleted), cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPost("{id}/add-studio-tags")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Guid>> AddStudioTags([FromRoute] Guid id, AddTagsToStudioCommand command, CancellationToken cancellationToken)
        {
            if (id != command.StudioId)
            {
                return BadRequest("Id in the url doesn't match the id in the body");
            }
            var result = await messageBus.InvokeAsync<Result<Guid>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPost("{id}/add-employee-details")]
        [ProducesResponseType(typeof(List<Guid>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<Guid>>> AddStudioTags([FromRoute] Guid id, AddEmployeesCommand command, CancellationToken cancellationToken)
        {
            if (id != command.StudioId)
            {
                return BadRequest("Id in the url doesn't match the id in the body");
            }
            var result = await messageBus.InvokeAsync<Result<List<Guid>>>(command, cancellationToken);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpGet("{id}/dashboard")]
        [ProducesResponseType(typeof(GetStudioDetailsByIdResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<GetStudioDetailsByIdResponse>> GetStudioDashboard([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<GetStudioDetailsByIdResponse>>(new GetStudioDetailsByIdQuery(id), cancellationToken);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpGet("inquiry-list/{statusId}")]
        [ProducesResponseType(typeof(GetInquiriesQueryResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<GetInquiriesQueryResponse>> GetInquiriesForStudio([FromRoute] InquiryStatus statusId, [FromQuery] PaginationOptions? paginationOptions, 
            [FromQuery] InquiryFilterOptions? filterOptions, CancellationToken cancellationToken)
        {
            var result = await messageBus.InvokeAsync<Result<GetInquiriesQueryResponse>>(new GetInquiriesQuery(statusId, paginationOptions ?? new(), filterOptions), cancellationToken);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpGet("inquiries/{inquiryId}")]
        public async Task<IActionResult> GetInquiryDetails([FromRoute] Guid inquiryId)
        {
            var query = new GetInquiryDetailsQuery(inquiryId);
            var result = await messageBus.InvokeAsync<Ardalis.Result.Result<GetInquiryDetailsResponse>>(query);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Status == Ardalis.Result.ResultStatus.NotFound)
            {
                return NotFound(result.Errors);
            }

            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpPatch("inquiries/{inquiryId}/respond")]
        public async Task<ActionResult<Guid>> RespondToInquiry([FromRoute] Guid inquiryId, [FromBody] AcceptInquiryCommand command, CancellationToken cancellationToken)
        {
            if (command.InquiryId != inquiryId)
                return BadRequest("Inquiry id on the url is not the same as the one in body.");

            var result = await messageBus.InvokeAsync<Ardalis.Result.Result<Guid>>(command, cancellationToken);
            return result.ToActionResult(this);
        }
    }
}
