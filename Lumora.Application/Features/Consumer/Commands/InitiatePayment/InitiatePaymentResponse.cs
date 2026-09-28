namespace Lumora.Application.Features.Consumer.Commands.InitiatePayment;

public record InitiatePaymentResponse(
    PaymentStudioInfoDto Studio,
    PaymentEventInfoDto Event,
    PaymentCostBreakdownDto Cost,
    UpiPaymentDto Upi
);

public record InquiryPaymentDetailsDto(
    Guid InquiryId,
    string StudioName,
    string? LogoUrl,
    decimal AverageRating,
    int ReviewCount,
    string StudioLocation,
    List<string> Tags,
    string EventTitle,
    DateOnly EventDate,
    decimal Duration,
    string EventLocation,
    decimal QuotedAmount,
    string? PayoutUpiId
);

public record PaymentStudioInfoDto(string Name, string? LogoUrl, decimal Rating, int ReviewCount, string Location, List<string> Tags);
public record PaymentEventInfoDto(string Title, DateTime Date, decimal Duration, string Location);
public record PaymentCostBreakdownDto(decimal ServiceFee, decimal PlatformFee, decimal TotalAmount);
public record UpiPaymentDto(string UpiId, string QrCodeBase64, string OrderId);