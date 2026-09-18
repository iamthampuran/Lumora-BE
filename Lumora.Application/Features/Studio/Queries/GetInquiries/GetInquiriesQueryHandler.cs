using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Queries.GetInquiries;

public class GetInquiriesQueryHandler(ILogger<GetInquiriesQueryHandler> logger, ICurrentUserService currentUserService, IInquiryRepository inquiryRepository)
{
    public async Task<Result<GetInquiriesQueryResponse>> Handle(GetInquiriesQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetInquiriesQuery));

        var currentUser = currentUserService.GetCurrentUserDetails();
        if (currentUser == null || currentUser.StudioId is null) return Result.Unauthorized("User not found with the data");

        var response = await inquiryRepository.GetStudioInquiriesAsync(currentUser.StudioId.Value, query.Status, query.PaginationOptions, query.FilterOptions, cancellationToken);
        return Result.Success(response);
    }
}
