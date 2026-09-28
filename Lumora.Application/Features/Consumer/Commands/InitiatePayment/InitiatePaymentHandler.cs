using Ardalis.Result;
using Lumora.Application.Configuration;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Payments;
using Lumora.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QRCoder;

namespace Lumora.Application.Features.Consumer.Commands.InitiatePayment;

public class InitiatePaymentHandler(ICurrentUserService currentUserService, ILogger<InitiatePaymentHandler> logger, IUnitOfWork unitOfWork, IGenericRepository<Payment> paymentRepository,
    IInquiryRepository inquiryRepository, IPaymentQueue paymentQueue, IOptions<AppSettingsConfiguration> appSettings, IMinioService minioService)
{
    public async Task<Result<InitiatePaymentResponse>> Handle(InitiatePaymentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Initiating simulated Razorpay UPI payment for Inquiry {InquiryId}", command.InquiryId);

        var userDetails = currentUserService.GetCurrentUserDetails();
        if (userDetails is null)
            return Result.Unauthorized("User is not logged in");

        if (userDetails.ConsumerId == null)
            return Result.Forbidden("User is not a consumer");

        var inquiry = await inquiryRepository.GetFirstAsync(i => i.Id == command.InquiryId && i.ConsumerId == userDetails.ConsumerId.Value,
            null,
            [i => i.Event, i => i.Studio, i => i.Studio.Tags],
            false,
            cancellationToken);

        if (inquiry is null)
            return Result.NotFound("Inquiry not found");

        if (inquiry.Status != InquiryStatus.Accepted)
            return Result.Error("Payment can only be initiated for accepted inquiries");

        decimal serviceFee = (inquiry.QuotedAmount.HasValue && inquiry.QuotedAmount.Value > 0)
            ? inquiry.QuotedAmount.Value
            : inquiry.Studio.MinPrice;

        var platformPercent = appSettings.Value.Razorpay.PlatformFeePercentage;
        var platformFee = Math.Round(serviceFee * platformPercent, 2);
        var totalAmount = serviceFee + platformFee;

        var merchantUpiId = appSettings.Value.Razorpay.MerchantUpiId ?? "lumora.razorpay@icici";
        var razorpayOrderId = $"order_sim_{Guid.NewGuid():N}";
        var razorpayQrId = $"qr_sim_{Guid.NewGuid():N}";

        var upiUri = $"upi://pay?pa={merchantUpiId}&pn={Uri.EscapeDataString(inquiry.Studio.StudioName)}&am={totalAmount:0.00}&tr={razorpayOrderId}&cu=INR";


        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(upiUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        string qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";

        var payment = new Payment
        {
            InquiryId = inquiry.Id,
            StudioId = inquiry.StudioId,
            ServiceFee = serviceFee,
            PlatformFee = platformFee,
            Amount = totalAmount,
            Status = PaymentStatus.Initiated,
            RazorPayOrderId = razorpayOrderId,
            RazorPayQrCodeId = razorpayQrId,
            InitiatedAt = DateTime.UtcNow
        };

        paymentRepository.Add(payment);

        inquiry.Event.Status = EventStatus.PaymentPending;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        paymentQueue.Enqueue(payment.Id, inquiry.Id);

        var studioInfo = new PaymentStudioInfoDto(
            inquiry.Studio.StudioName,
            inquiry.Studio.LogoUrl != null ? await minioService.GeneratePresignedUrlAsync(inquiry.Studio.LogoUrl) : null,
            inquiry.Studio.AverageRating ?? 0,
            inquiry.Studio.ReviewCount,
            inquiry.Studio.Location?.LocationName ?? string.Empty,
            inquiry.Studio.Tags.Select(t => t.Tag?.Name ?? string.Empty).Where(t => !string.IsNullOrEmpty(t)).ToList()
        );

        var eventInfo = new PaymentEventInfoDto(
            inquiry.Event.Title,
            inquiry.Event.EventDate.ToDateTime(TimeOnly.MinValue),
            inquiry.Event.Duration,
            inquiry.Event.Location?.LocationName ?? string.Empty
        );

        var costBreakdown = new PaymentCostBreakdownDto(serviceFee, platformFee, totalAmount);
        var upiDetails = new UpiPaymentDto(merchantUpiId, qrCodeBase64, razorpayOrderId);

        return Result.Success(new InitiatePaymentResponse(studioInfo, eventInfo, costBreakdown, upiDetails));
    }
}
