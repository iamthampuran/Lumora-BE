using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.AcceptInquiry;

public class AcceptInquiryHandler(IInquiryRepository inquiryRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<AcceptInquiryHandler> logger)
{
    public async Task<Result<Guid>> Handle(AcceptInquiryCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(AcceptInquiryCommand));

        var currentUser = currentUserService.GetCurrentUserDetails();
        if (currentUser == null)
        {
            return Result.Unauthorized("User not logged");
        }

        if (currentUser.StudioId == null)
        {
            return Result.Forbidden("User doesn't have the permission to access this");
        }

        var inquiry = await inquiryRepository.GetByIdAsync(command.InquiryId, cancellationToken);
        if (inquiry == null)
        {
            return Result.NotFound("Inquiry was not found");
        }

        if (inquiry.StudioId != currentUser.StudioId.Value)
        {
            return Result.Forbidden("User doesn't have the permission to access this inquiry");
        }

        if (inquiry.Status != Domain.Enums.InquiryStatus.Submitted)
        {
            return Result.Error("The following inquiry can only be move to accept or reject from submit state!!");
        }

        if (command.IsAccepted)
            inquiry.Status = Domain.Enums.InquiryStatus.Accepted;
        else
        {
            inquiry.Status = Domain.Enums.InquiryStatus.Rejected;
            inquiry.RejectionStatus = command.RejectedMessage;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(inquiry.Id);
    }
}
