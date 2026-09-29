using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.AssignInquiryEmployees;

public class AssignInquiryEmployeesCommandValidator : AbstractValidator<AssignInquiryEmployeesCommand>
{
    public AssignInquiryEmployeesCommandValidator()
    {
        RuleFor(x => x.InquiryId)
            .NotEmpty()
            .WithMessage("Inquiry id is required.");

        RuleFor(x => x.EmployeeIds)
            .NotNull()
            .WithMessage("Employee ids are required.")
            .Must(x => x != null && x.Any())
            .WithMessage("At least one employee must be assigned.")
            .Must(x => x != null && x.Distinct().Count() == x.Count())
            .WithMessage("Duplicate employee ids are not allowed.");

        RuleForEach(x => x.EmployeeIds)
            .NotEmpty()
            .WithMessage("Employee id is required.");
    }
}