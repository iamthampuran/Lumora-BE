using FluentValidation;

namespace Lumora.Application.Features.Auth.Commands.CreateConsumer;

public class CreateStudioCommandValidator : AbstractValidator<CreateConsumerCommand>
{
    public CreateStudioCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number cannot be empty")
            .Matches(@"^(\+91|91)?[6-9]\d{9}$")
            .WithMessage("Please enter a valid Indian phone number.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}
