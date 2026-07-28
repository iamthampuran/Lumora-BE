using FluentValidation;

namespace Lumora.Application.Features.Auth.Commands.CreateStudio;

public class CreateStudioCommandValidator : AbstractValidator<CreateStudioCommand>
{
    public CreateStudioCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required")
            .Matches(@"^(\+91|91)?[6-9]\d{9}$")
            .WithMessage("Please enter a valid Indian phone number.");

        RuleFor(x => x.MaxPrice)
            .NotEmpty()
            .InclusiveBetween(0, 100000)
            .WithMessage("Max price must be between 0 and 100k");

        RuleFor(x => x.MinPrice)
            .NotEmpty()
            .InclusiveBetween(0, 100000)
            .WithMessage("Min price must be between 0 and 100k");

        RuleFor(x => x.StudioName)
            .MaximumLength(300)
            .WithMessage("Studio name cannot exceed 300 characters")
            .NotEmpty()
            .WithMessage("Studio name cannot be empty");

    }
}
