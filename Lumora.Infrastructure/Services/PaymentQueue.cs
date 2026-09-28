using Lumora.Application.Contracts.Services;
using System.Threading.Channels;

namespace Lumora.Infrastructure.Services;

public class PaymentQueue : IPaymentQueue
{
    private readonly Channel<(Guid paymentId, Guid inquiryId)> _channel = Channel.CreateUnbounded<(Guid paymentId, Guid inquiryId)> ();

    public void Enqueue(Guid paymentId, Guid inquiryId)
    {
        _channel.Writer.TryWrite((paymentId, inquiryId));
    }

    public IAsyncEnumerable<(Guid paymentId,Guid inquiryId)> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
