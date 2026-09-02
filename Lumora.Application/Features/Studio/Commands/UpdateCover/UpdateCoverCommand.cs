namespace Lumora.Application.Features.Studio.Commands.UpdateCover;

public record UpdateCoverCommand(Stream File, Guid StudioId, string ContentType);

