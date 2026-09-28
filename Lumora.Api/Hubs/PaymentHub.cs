using Microsoft.AspNetCore.SignalR;

namespace Lumora.Api.Hubs;

public class PaymentHub : Hub
{
    public async Task JoinInquiryGroup(Guid inquiryId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Inquiry_{inquiryId}");
    }

    public async Task LeaveInquiryGroup(Guid inquiryId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Inquiry_{inquiryId}");
    }
}
