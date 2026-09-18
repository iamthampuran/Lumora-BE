using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Commands.DeleteEvent;

public class DeleteEventCommandHandler(ILogger<DeleteEventCommandHandler> logger, IEventRepository eventRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
{
    public async Task<Result<Guid>> Handle(DeleteEventCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(DeleteEventCommand));

        var eventData = await eventRepository.GetFirstAsync(e => e.Id == command.Id, null, [e => e.Inquiries], false, cancellationToken);
        if (eventData == null)
        {
            return Result.NotFound("Event with the id was not found");
        }

        var userData = currentUserService.GetCurrentUserDetails();
        if (userData == null || (userData.ConsumerId != null && userData.ConsumerId != eventData.ConsumerId))
        {
            return Result.Unauthorized("User not allowed to delete");
        }

        if (eventData.Inquiries.Any(i => i.Status == Domain.Enums.InquiryStatus.Confirmed) || DeleteRestrictingStatus().Contains(eventData.Status))
            return Result.Error("An event with the current status cannot be deleted");

        eventData.DeleteEntity();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(command.Id);
    }


    private List<EventStatus> DeleteRestrictingStatus()
    {
        return [EventStatus.Paid, EventStatus.AlbumReview, EventStatus.Complete, EventStatus.InProgress, EventStatus.RequestChanges];
    }
}
