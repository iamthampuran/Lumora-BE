using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using ZXing;
using ZXing.QrCode;
using Result = Ardalis.Result.Result;

namespace Lumora.Application.Features.Studio.Commands.AddPaymentInformation;

public class AddPaymentInformationHandler(ILogger<AddPaymentInformationCommand> logger, IStudioRepository studioRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
{
    public async Task<Result<Guid>> Handle(AddPaymentInformationCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(AddPaymentInformationCommand));

        var currentUser = currentUserService.GetCurrentUserDetails();
        if (currentUser == null)
            return Result.Unauthorized("User must be logged in");

        if (currentUser.StudioId == null)
            return Result.Forbidden("User does not have access to this task");

        var studio = await studioRepository.GetByIdAsync(currentUser.StudioId.Value, cancellationToken);
        if (studio == null)
            return Result.NotFound("User not found with the studio id");

        var upiId = ExtractUPIFromQRCode(command.QrInformation!.FileStream);
        if (upiId == null)
            return Result.Error("Error processing the image");

        studio.PayoutUpiId = command.UpiId == null ? upiId : command.UpiId;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(studio.Id);   
        
    }

    private string? ExtractUPIFromQRCode(Stream stream)
    {
        try
        {
            stream.Seek(0, SeekOrigin.Begin);

            using (var image = Image.Load(stream))
            {
                // Convert to grayscale and extract pixel data
                var width = image.Width;
                var height = image.Height;
                var pixels = new byte[width * height];

                var grayscale = image.CloneAs<SixLabors.ImageSharp.PixelFormats.L8>();
                grayscale.CopyPixelDataTo(pixels);

                // Use BarcodeReaderGeneric instead of BarcodeReader<T>
                var reader = new BarcodeReaderGeneric();

                // Use PlanarYUVLuminanceSource for a 1-byte-per-pixel (L8) grayscale array
                var luminanceSource = new PlanarYUVLuminanceSource(pixels, width, height, 0, 0, width, height, false);
                var result = reader.Decode(luminanceSource);

                if (result == null)
                    return null;

                // Extract UPI ID from QR data
                var upiData = result.Text;
                if (upiData.StartsWith("upi://", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(upiData);
                    return System.Web.HttpUtility.ParseQueryString(uri.Query)["pa"];
                }

                return upiData.Contains("@") ? upiData : null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to extract UPI ID from QR code");
            return null;
        }
    }

}
