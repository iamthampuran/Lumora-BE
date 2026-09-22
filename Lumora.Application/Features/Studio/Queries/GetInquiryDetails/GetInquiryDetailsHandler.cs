using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Queries.GetInquiryDetails;

public class GetInquiryDetailsHandler(IInquiryRepository inquiryRepository, ICurrentUserService currentUserService, ILogger<GetInquiryDetailsHandler> logger)
{
    public async Task<Result<GetInquiryDetailsResponse>> Handle(GetInquiryDetailsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetInquiryDetailsQuery));

        var currentUser = currentUserService.GetCurrentUserDetails();
        if (currentUser == null || currentUser.StudioId == null)
            return Result.Forbidden();

        var inquiryDetails = await inquiryRepository.GetStudioInquiryDetailsAsync(currentUser.StudioId.Value, query.InquiryId, cancellationToken);

        if (inquiryDetails == null)
        {
            return Result.NotFound("Inquiry with the id was not found");
        }

        return Result.Success(inquiryDetails);
    }
}
