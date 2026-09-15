using FluentValidation;

namespace Lumora.Application.Features.Consumer.Commands.UpdateEvent;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Event ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.EventDate)
            .NotEmpty()
            .WithMessage("Event date is required")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Event date cannot be in the past");

        RuleFor(x => x.Budget)
            .GreaterThan(0)
            .WithMessage("Budget must be greater than zero")
            .LessThanOrEqualTo(100000)
            .WithMessage("Budget cannot exceed 100,000");

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than zero")
            .LessThanOrEqualTo(24)
            .WithMessage("Duration cannot exceed 24 hours");

        RuleFor(x => x.Location)
            .NotNull()
            .WithMessage("Location is required");

        RuleFor(x => x.ConsumerId)
            .NotEmpty()
            .WithMessage("Consumer ID is required");

        RuleFor(x => x)
            .Must(x =>
                (x.EventCategoryId is null) !=
                (string.IsNullOrEmpty(x.CustomEventCategory)))
            .WithMessage("Either provide an EventCategoryId or a CustomEventCategory, but not both");

        RuleFor(x => x.TagIds)
            .NotEmpty()
            .WithMessage("At least one tag is required");
    }
}
