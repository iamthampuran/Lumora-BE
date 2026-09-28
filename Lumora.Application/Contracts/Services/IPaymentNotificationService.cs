namespace Lumora.Application.Contracts.Services;

public interface IPaymentNotificationService
{
    Task NotifyConfirmedAsync(Guid inquiryId, Guid paymentId, string transactionId, DateTime? PaidAt);
    Task NotifyFailedAsync(Guid inquiryId, Guid paymentId, string reason);
}
