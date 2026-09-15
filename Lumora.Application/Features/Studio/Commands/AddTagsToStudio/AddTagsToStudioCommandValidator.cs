using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.AddTagsToStudio;

public class AddTagsToStudioCommandValidator : AbstractValidator<AddTagsToStudioCommand>
{
    public AddTagsToStudioCommandValidator()
    {
        RuleFor(x => x.StudioId)
            .NotEmpty()
            .WithMessage("Studio ID is required");

        RuleFor(x => x.TagIds)
            .NotEmpty()
            .WithMessage("At least one tag ID is required")
            .Must(x => x.All(tagId => tagId != Guid.Empty))
            .WithMessage("All tag IDs must be valid non-empty GUIDs")
            .When(x => x.TagIds != null && x.TagIds.Any());

        RuleFor(x => x.CustomTagDetails)
            .ForEach(x => x.NotEmpty().WithMessage("Custom tag details cannot be empty"))
            .When(x => x.CustomTagDetails != null && x.CustomTagDetails.Any());
    }
}
