namespace Lumora.Application.Features.Auth.Commands.CreateConsumer;

public record CreateConsumerCommand(Guid UserId, string FullName, string PhoneNumber, string? PhotoUrl, string? Bio);
