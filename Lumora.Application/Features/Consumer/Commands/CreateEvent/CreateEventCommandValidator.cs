using FluentValidation;

namespace Lumora.Application.Features.Consumer.Commands.CreateEvent;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.EventDate)
            .NotEmpty().WithMessage("Event date is required.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Now)).WithMessage("Event date cannot be in the past.");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero.")
            .LessThanOrEqualTo(100000).WithMessage("Budget cannot exceed 100,000.");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than zero.")
            .LessThanOrEqualTo(24).WithMessage("Duration cannot exceed 24 hours.");

        RuleFor(x => x.Location)
            .NotNull().WithMessage("Location is required.");

        RuleFor(x => x)
                    .Must(x =>
                        (x.EventCategoryId is null) !=
                        (x.CustomEventCategory is null))
                    .WithMessage(
                        "Either EventCategoryId or CustomEventCategory must be provided, but not both.");
    }
}
