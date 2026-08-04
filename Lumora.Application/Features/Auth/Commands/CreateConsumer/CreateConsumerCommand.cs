namespace Lumora.Application.Features.Auth.Commands.CreateConsumer;

public class CreateConsumerCommand
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public FileDetails? FileDetails { get; init; }
    public string? Bio { get; init; }
}

public record CreateConsumerDto(Guid UserId, string FullName, string PhoneNumber, string? Bio);

public record FileDetails(Stream fileStream, string contentType);
