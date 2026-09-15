using FluentValidation;

namespace Lumora.Application.Features.Consumer.Commands.CreateInquiry;

public class CreateInquiryCommandValidator : AbstractValidator<CreateInquiryCommand>
{
    public CreateInquiryCommandValidator()
    {
        RuleFor(x => x.eventId)
            .NotEmpty()
            .WithMessage("Event ID is required");

        RuleFor(x => x.studioId)
            .NotEmpty()
            .WithMessage("Studio ID is required");

        RuleFor(x => x.consumerId)
            .NotEmpty()
            .WithMessage("Consumer ID is required");

        RuleFor(x => x.message)
            .MaximumLength(500)
            .WithMessage("Message cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.message));

        RuleFor(x => x.quotedAmount)
            .GreaterThan(0)
            .WithMessage("Quoted amount must be greater than zero")
            .When(x => x.quotedAmount.HasValue);
    }
}
