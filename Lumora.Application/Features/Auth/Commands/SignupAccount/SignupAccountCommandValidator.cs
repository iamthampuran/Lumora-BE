using FluentValidation;

namespace Lumora.Application.Features.Auth.Commands.SignupAccount;

public class SignupAccountCommandValidator : AbstractValidator<SignupAccountCommand>
{
    public SignupAccountCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty or null")
            .EmailAddress()
            .WithMessage("Please provide a valid email");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty or null")
            .MinimumLength(8)
            .WithMessage("Password must be atleast 8 characters long")
            .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
                .WithMessage("Password must contain at least one digit.")
            .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("The role cannot be null");
    }
}
