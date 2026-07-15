namespace Lumora.Application.Contracts.Services;

public interface IMinioService
{
    Task<(string fileKey, string presignedUrl, string fileName)> UploadFileAsync(Stream fileStream, string entityType, string entityId, string imageType, string fileName, CancellationToken cancellationToken);
    Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken);
    Task<string> GeneratePresignedUrlAsync(string fileKey, int expirtationMinutes = 60);
}
