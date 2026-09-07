namespace Lumora.Application.Features.Studio.Commands.AddTagsToStudio;

public record AddTagsToStudioCommand(Guid StudioId, IEnumerable<Guid> TagIds, IEnumerable<string>? CustomTagDetails);

