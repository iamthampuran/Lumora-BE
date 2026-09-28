namespace Lumora.Application.Contracts.Services;

public interface IPaymentQueue
{
    void Enqueue(Guid paymentId, Guid inquiryId);
}
