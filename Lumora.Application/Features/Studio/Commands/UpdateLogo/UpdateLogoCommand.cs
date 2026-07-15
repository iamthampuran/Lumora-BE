namespace Lumora.Application.Features.Studio.Commands.UpdateLogo;

public record UpdateLogoCommand(Stream File, Guid StudioId, string ContentType);
