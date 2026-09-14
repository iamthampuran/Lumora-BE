using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.AddPortfolioImage;

public class AddPortfolioImageCommandValidator : AbstractValidator<AddPortfolioImageCommand>
{
    public AddPortfolioImageCommandValidator()
    {
        RuleFor(x => x.StudioId)
            .NotEmpty()
            .WithMessage("Studio ID is required");

        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File stream is required");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required")
            .Must(x => x.StartsWith("image/"))
            .WithMessage("Content type must be an image format");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order must be greater than or equal to 0");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));
    }
}
