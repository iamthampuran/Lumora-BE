namespace Lumora.Application.Features.Studio.Commands.AcceptInquiry;

public record AcceptInquiryCommand(bool IsAccepted, Guid InquiryId, string? RejectedMessage);
