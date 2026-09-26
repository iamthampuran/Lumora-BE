namespace Lumora.Application.Features.Studio.Commands.AddPaymentInformation;

public record AddPaymentInformationCommand(string? UpiId, QrInformation? QrInformation);
public record QrInformation(Stream? FileStream, string? Title, string? ContentType);

