using FluentValidation;

namespace Lumora.Application.Features.Studio.Commands.AddPaymentInformation;

public class AddPaymentInformationValidator : AbstractValidator<AddPaymentInformationCommand>
{
    public AddPaymentInformationValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.UpiId) || x.QrInformation is not null)
            .WithMessage("Either UPI ID or QR code information must be provided.");
    }
}
