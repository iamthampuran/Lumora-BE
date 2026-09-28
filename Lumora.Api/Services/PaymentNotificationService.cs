using Lumora.Api.Hubs;
using Lumora.Application.Contracts.Services;
using Microsoft.AspNetCore.SignalR;

namespace Lumora.Api.Services;

public class PaymentNotificationService(IHubContext<PaymentHub> hubContext) : IPaymentNotificationService
{
    public async Task NotifyConfirmedAsync(Guid inquiryId, Guid paymentId, string transactionId, DateTime? paidAt)
    {
        await hubContext.Clients.Group($"Inquiry_{inquiryId}").SendAsync("PaymentConfirmed", new
        {
            inquiryId,
            paymentId,
            status = "Confirmed",
            transactionId,
            paidAt
        });
    }

    public async Task NotifyFailedAsync(Guid inquiryId, Guid paymentId, string reason)
    {
        await hubContext.Clients.Group($"Inquiry_{inquiryId}").SendAsync("PaymentFailed", new
        {
            inquiryId,
            paymentId,
            reason
        });
    }
}
