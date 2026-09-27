using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Payments;
using Lumora.Domain.Enums;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace Lumora.Application.Features.Consumer.Commands.InitiatePayment;

public class InitiatePaymentHandler(ICurrentUserService currentUserService, ILogger<InitiatePaymentHandler> logger, IUnitOfWork unitOfWork, IMinioService minioService, IGenericRepository<Payment> paymentRepository,
    IInquiryRepository inquiryRepository)
{
    public async Task<Result<InitiatePaymentResponse>> Handle(InitiatePaymentCommand command, CancellationToken cancellationToken)
    {
        var userDetails = currentUserService.GetCurrentUserDetails();
        if (userDetails is null)
            return Result.Unauthorized("User is not logged in");

        if (userDetails.ConsumerId == null)
            return Result.Forbidden("User is not a consumer");

        var paymentDetails = await inquiryRepository.GetInquiryPaymentDetailsAsync(command.InquiryId, userDetails.ConsumerId.Value, cancellationToken);

        if (paymentDetails == null)
            return Result.NotFound("Inquiry not found or not accessible by the consumer");

        var platformFee = paymentDetails.QuotedAmount * 0.05m; // 5% platform fee
        var totalAmount = paymentDetails.QuotedAmount + platformFee;

        var orderId = $"ORDER_{Guid.NewGuid().ToString("N")[..10].ToUpper()}";

        var upiUri = $"upi://pay?pa={paymentDetails.PayoutUpiId}&pn={Uri.EscapeDataString(paymentDetails.StudioName)}&am={totalAmount:0.00}&tr={orderId}&cu=INR";


        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(upiUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);

        var qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";


        var payment = new Payment
        {
            InquiryId = paymentDetails.InquiryId,
            Amount = totalAmount,
            Status = PaymentStatus.Pending,
            RazorPayOrderId = orderId,
            ServiceFee = paymentDetails.QuotedAmount * 0.01m,
            PlatformFee = platformFee,

        };
        return Result.Success();

    }
}
