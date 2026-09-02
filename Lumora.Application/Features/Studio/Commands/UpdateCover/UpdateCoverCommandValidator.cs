using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.UpdateCover;

public class UpdateCoverCommandValidator : AbstractValidator<UpdateCoverCommand>
{
    public UpdateCoverCommandValidator()
    {
        RuleFor(x => x.StudioId)
           .NotNull()
           .WithMessage("Studio id cannot be null");

        RuleFor(x => x.File)
            .NotNull()
            .NotEmpty()
            .WithMessage("File cannot be empty");

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(5 * 1024 * 1024)
            .WithMessage("Max file size is 2MB");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Cannot find the content type");
    }
}
