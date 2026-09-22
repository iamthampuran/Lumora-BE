using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Queries.GetStudioMembers;

public class GetStudioMembersHandler(IStudioRepository studioRepository, ICurrentUserService currentUserService, ILogger<GetStudioMembersHandler> logger)
{
    public async Task<Result<IEnumerable<GetStudioMembersResponse>>> Handle(GetStudioMembersQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(query));

        var userDetails = currentUserService.GetCurrentUserDetails();
        if (userDetails == null)
            return Result.Unauthorized("User not logged in");

        if (userDetails.StudioId == null)
            return Result.Forbidden("User is not a valid studio");

        var studio = await studioRepository.GetFirstAsync(
            s => s.Id == userDetails.StudioId.Value,
            null,
            [s => s.Employees],
            true,
            cancellationToken);

        if (studio == null)
            return Result.NotFound("Studio not found");

        var returnDetails = studio.Employees.Select(e => new GetStudioMembersResponse(
            e.Id,
            e.FullName,
            e.Email,
            e.Phone,
            e.EmployeeRole
        ));

        return Result.Success(returnDetails);
    }
}
