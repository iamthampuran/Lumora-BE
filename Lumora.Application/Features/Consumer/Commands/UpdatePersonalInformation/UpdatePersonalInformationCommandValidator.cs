using FluentValidation;

namespace Lumora.Application.Features.Consumer.Commands.UpdatePersonalInformation;

public class UpdatePersonalInformationCommandValidator : AbstractValidator<UpdatePersonalInformationCommand>
{
    public UpdatePersonalInformationCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^(\+91|91)?[6-9]\d{9}$")
            .WithMessage("Please enter a valid Indian phone number.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .WithMessage("Bio cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Bio));
    }
}
