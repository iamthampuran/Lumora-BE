using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Event;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Commands.CreateInquiry;

public class CreateInquiryCommandHandler(IInquiryRepository inquiryRepository, ILogger<CreateInquiryCommandHandler> logger, IUnitOfWork unitOfWork, IEventRepository eventRepository)
{
    public async Task<Result<Guid>> Handle(CreateInquiryCommand command, CancellationToken cancellationToken)
    {
        var eventData = await eventRepository.GetByIdAsync(command.eventId, cancellationToken);

        if (eventData == null)
            return Result.NotFound("Event not found.");

        var doesInquiryExist = await inquiryRepository.AnyAsync(i => i.EventId == command.eventId && i.ConsumerId == command.consumerId && i.StudioId == command.studioId, cancellationToken);
        if (doesInquiryExist)
        {
            return Result.Error("An inquiry for this event already exists for this studio.");
        }

        var inquiry = new Inquiry()
        {
            EventId = command.eventId,
            StudioId = command.studioId,
            ConsumerId = command.consumerId,
            Message = command.message,
            QuotedAmount = command.quotedAmount
        };

        inquiryRepository.Add(inquiry);

        eventData.Status = Domain.Enums.EventStatus.InquiryInProgress;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(inquiry.Id);
    }
}
