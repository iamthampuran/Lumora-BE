using Lumora.Application.Configuration;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Payments;
using Lumora.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lumora.Infrastructure.Services;

public class PaymentBackgroundService(PaymentQueue queue, IServiceScopeFactory scopeFactory, IOptions<AppSettingsConfiguration> options, 
    Microsoft.Extensions.Logging.ILogger<PaymentBackgroundService> logger)
    : BackgroundService
{
    private static readonly Random randomGenerator = new();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Payment simulator is starting..");

        await foreach (var (paymentId, inquiryId) in queue.ReadAllAsync(cancellationToken))
        {
            _ = ProcessPaymentAsync(paymentId, inquiryId, cancellationToken);
        }
    }

    private async Task ProcessPaymentAsync(Guid paymentId, Guid inquiryId, CancellationToken cancellationToken)
    {
        try
        {
            (var minDelay, var maxDelay) = (options.Value.Razorpay.MinSimulationDelaySeconds, options.Value.Razorpay.MaxSimulationDelaySeconds);
            
            await Task.Delay(TimeSpan.FromSeconds(randomGenerator.Next(minDelay, maxDelay + 1)), cancellationToken);

            using var scope = scopeFactory.CreateScope();
            var paymentRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Payment>>();
            var inquiryRepository = scope.ServiceProvider.GetRequiredService<IInquiryRepository>();
            var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<IPaymentNotificationService>();

            var payment = await paymentRepository.GetFirstAsync(p => p.Id == paymentId,  includes: [p => p.Inquiry], disableTracking: false, cancellationToken: cancellationToken);
            if (payment == null || payment.Status != PaymentStatus.Initiated) return;   //replace with actual code for razorpay waiting for payment.

            // 99% Success, 1% Failure
            bool isSuccess = randomGenerator.Next(1, 101) <= 99;

            if (isSuccess)
            {
                payment.Status = PaymentStatus.Completed;
                payment.CompletedAt = DateTime.UtcNow;
                payment.TransactionId = $"pay_sim_{Guid.NewGuid():N}";
                payment.RazorePaySignature = $"sig_sim_{Guid.NewGuid():N}";

                var inquiries = await inquiryRepository.GetAsync(i => i.EventId == payment.Inquiry.EventId, cancellationToken);

                foreach (var inquiryData in inquiries.Where(i => i.Id != payment.InquiryId).ToList())
                {
                    inquiryData.Status = InquiryStatus.Cancelled;
                }

                //var inquiry = await inquiryRepository.GetByIdAsync(payment.InquiryId, cancellationToken);
                var inquiry = inquiries.FirstOrDefault(i => i.Id == payment.InquiryId);
                if (inquiry != null) inquiry.Status = InquiryStatus.Confirmed;

                var eventData = await eventRepository.GetByIdAsync(inquiry!.EventId, cancellationToken);
                if (eventData != null)
                {
                    eventData.Status = EventStatus.Paid;
                    eventData.SelectedStudioId = payment.StudioId;
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Simulated Payment SUCCESS for PaymentId {PaymentId}", paymentId);

                await notificationService.NotifyConfirmedAsync(inquiryId, payment.Id, payment.TransactionId, payment.CompletedAt);
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = "Payment failed: Bank servers declined the transaction (Simulated 1% edge case).";

                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogWarning("Simulated Payment FAILED for PaymentId {PaymentId}", paymentId);

                await notificationService.NotifyFailedAsync(inquiryId, payment.Id, payment.FailureReason);
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment for paymentId {paymentId}", paymentId);
        }
    }
}
