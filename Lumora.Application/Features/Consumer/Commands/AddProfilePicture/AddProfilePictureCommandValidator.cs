using FluentValidation;

namespace Lumora.Application.Features.Consumer.Commands.AddProfilePicture;

public class AddProfilePictureCommandValidator : AbstractValidator<AddProfilePictureCommand>
{
    public AddProfilePictureCommandValidator()
    {
        RuleFor(x => x.fileStream)
            .NotNull()
            .WithMessage("File stream is required");

        RuleFor(x => x.contentType)
            .NotEmpty()
            .WithMessage("Content type is required")
            .Must(x => x.StartsWith("image/"))
            .WithMessage("Content type must be an image format");

        RuleFor(x => x.consumerId)
            .NotEmpty()
            .WithMessage("Consumer ID is required");
    }
}
