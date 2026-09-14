using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.AddEmployees;

public class AddEmployeesCommandValidator : AbstractValidator<AddEmployeesCommand>
{
    public AddEmployeesCommandValidator()
    {
        RuleFor(x => x.StudioId)
            .NotEmpty()
            .WithMessage("Studio ID is required");

        RuleFor(x => x.EmployeeDetails)
            .NotEmpty()
            .WithMessage("At least one employee is required")
            .Must(x => x.All(e => !string.IsNullOrEmpty(e.Name)))
            .WithMessage("All employees must have a name");

        RuleForEach(x => x.EmployeeDetails)
            .SetValidator(new EmployeeDetailValidator());
    }
}

public class EmployeeDetailValidator : AbstractValidator<EmployeeDetail>
{
    public EmployeeDetailValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Employee name is required")
            .MaximumLength(100)
            .WithMessage("Employee name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Employee email is required")
            .EmailAddress()
            .WithMessage("Employee email must be valid");

        RuleFor(x => x.Phonenumber)
            .NotEmpty()
            .WithMessage("Employee phone number is required")
            .Matches(@"^(\+91|91)?[6-9]\d{9}$")
            .WithMessage("Please enter a valid Indian phone number");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Employee role is required")
            .MaximumLength(50)
            .WithMessage("Employee role cannot exceed 50 characters");
    }
}
