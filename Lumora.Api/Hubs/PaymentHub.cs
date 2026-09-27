using Microsoft.AspNetCore.SignalR;

namespace Lumora.Api.Hubs;

public class PaymentHub : Hub
{
    public async Task MonitorInquiryPayment(Guid inquiryId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Payment_Inquiry_{inquiryId}");
    }
}
