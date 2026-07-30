namespace Lumora.Application.Features.Consumer.Commands.AddProfilePicture;

public record AddProfilePictureCommand(Stream fileStream, Guid consumerId, string contentType);
