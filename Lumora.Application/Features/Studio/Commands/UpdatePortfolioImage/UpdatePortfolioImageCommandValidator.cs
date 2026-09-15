using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.UpdatePortfolioImage;

public class UpdatePortfolioImageCommandValidator : AbstractValidator<UpdatePortfolioImageCommand>
{
    public UpdatePortfolioImageCommandValidator()
    {
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage("Image ID is required");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.ContentType)
            .Must(x => string.IsNullOrEmpty(x) || x.StartsWith("image/"))
            .WithMessage("Content type must be an image format")
            .When(x => !string.IsNullOrEmpty(x.ContentType));

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order must be greater than or equal to 0")
            .When(x => x.Order.HasValue);

        RuleFor(x => x)
            .Must(x => x.File != null || x.IsDeleted || (!string.IsNullOrEmpty(x.Title) || x.Order.HasValue))
            .WithMessage("Either provide a file, mark as deleted, or update title/order");
    }
}
