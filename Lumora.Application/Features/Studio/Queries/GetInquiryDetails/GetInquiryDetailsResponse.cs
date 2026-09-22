namespace Lumora.Application.Features.Studio.Queries.GetInquiryDetails;

public record GetInquiryDetailsResponse(Guid InquiryId,
    string Status,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    decimal QuotedAmount,
    EventDetails Event,
    ConsumerDetails Consumer,
    PaymentSummary? Payment);

public record EventDetails(
    string Title,
    string Category,
    DateOnly Date,
    string Location,
    decimal Duration,
    decimal Budget,
    string SpecialRequirements,
    List<string> Tags
);

public record ConsumerDetails(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    int PriorBookings,
    string TierStatus
);

public record PaymentSummary(
    decimal ServiceFee,
    decimal PlatformFee,
    decimal TotalPaid,
    string TransactionStatus,
    string? TransactionId,
    string? PaymentMethod,
    DateTime? PaidAt
);