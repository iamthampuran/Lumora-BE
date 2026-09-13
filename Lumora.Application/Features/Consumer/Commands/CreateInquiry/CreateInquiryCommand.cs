namespace Lumora.Application.Features.Consumer.Commands.CreateInquiry;

public record CreateInquiryCommand(Guid eventId, Guid studioId, Guid consumerId, string? message, decimal? quotedAmount);
