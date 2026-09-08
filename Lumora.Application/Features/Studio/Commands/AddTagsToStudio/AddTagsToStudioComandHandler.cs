using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Studio;
using Lumora.Domain.Entities.Tag;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.AddTagsToStudio;

public class AddTagsToStudioComandHandler(ILogger<AddTagsToStudioComandHandler> logger, IUnitOfWork unitOfWork, ITagRepository tagRepository, 
    IGenericRepository<StudioTag> studioTagRepository, IStudioRepository studioRepository)
{
    public async Task<Result<Guid>> Handle(AddTagsToStudioCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling commant - {@command}", nameof(AddTagsToStudioCommand));
        var studio = studioRepository.GetByIdAsync(command.StudioId, cancellationToken);
        if (studio == null)
        {
            return Result.NotFound("Studio with the id was not found");
        }

        List<Guid> tagList = [.. command.TagIds];

        var invalidTagIdPassed = await tagRepository.AnyAsync(t => !command.TagIds.Contains(t.Id), cancellationToken);
        if (invalidTagIdPassed)
        {
            return Result.Error("1 or more tags selected does not exists");
        }

        if (command.CustomTagDetails != null)
        {
            foreach (var customTag in command.CustomTagDetails)
            {
                var newTag = new Tag(customTag);
                tagRepository.Add(newTag);
                tagList.Add(newTag.Id);
            }
        }

        var studioTags = new List<StudioTag>();
        foreach (var tag in tagList)
        {
            var studioTag = new StudioTag()
            {
                StudioProfile = command.StudioId,
                TagId = tag
            };
            studioTags.Add(studioTag);
        }

        studioTagRepository.AddRange(studioTags);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(command.StudioId);
    }
}
