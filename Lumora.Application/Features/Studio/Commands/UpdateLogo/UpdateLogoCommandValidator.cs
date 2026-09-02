using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.UpdateLogo;

public class UpdateLogoCommandValidator : AbstractValidator<UpdateLogoCommand>
{
    public UpdateLogoCommandValidator()
    {
        RuleFor(x => x.StudioId)
            .NotNull()
            .WithMessage("Studio id cannot be null");

        RuleFor(x => x.File)
            .NotNull()
            .NotEmpty()
            .WithMessage("File cannot be empty");

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(2 * 1024 * 1024)
            .WithMessage("Max file size is 2MB");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Cannot find the content type");
    }
}
