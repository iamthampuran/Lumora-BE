namespace Lumora.Application.Contracts.Services;

public interface IMinioService
{
    Task<(string fileKey, string presignedUrl, string fileName)> UploadFileAsync(Stream fileStream, string imageType, string entityId, string fileName,
        CancellationToken cancellationToken);
    Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken);
    Task<string> GeneratePresignedUrlAsync(string fileKey, int expirtationMinutes = 60);
    string BuildFilePath(string imageType, string entityId, string fileName);

}
