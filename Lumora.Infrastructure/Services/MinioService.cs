using Lumora.Application.Configuration;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Helpers;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Lumora.Infrastructure.Services;

public class MinioService(IMinioClient minioClient, IOptions<AppSettingsConfiguration> appsettingsConfiguration) : IMinioService
{
    private readonly string BucketName = appsettingsConfiguration.Value.Minio.BucketName;
    public async Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken)
    {
        await CreateOrCheckIfExistBucketAsync(cancellationToken);
        var deleteArgs = new RemoveObjectArgs()
            .WithBucket(BucketName)
            .WithObject(fileKey);

        await minioClient.RemoveObjectAsync(deleteArgs, cancellationToken);
        return true;
    }

    public async Task<string> GeneratePresignedUrlAsync(string filePath, int expirtationMinutes = 60)
    {
        var presignedObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(filePath)
            .WithExpiry(MessageConstants.PresignedUrlExpirationInMinutes);

        var presignedUrl = await minioClient.PresignedGetObjectAsync(presignedObjectArgs);
        return presignedUrl;
    }

    public async Task<(string fileKey, string presignedUrl, string fileName)> UploadFileAsync(Stream fileStream, string entityType, string entityId, string imageType, string filePath, 
        CancellationToken cancellationToken)
    {

        await CreateOrCheckIfExistBucketAsync(cancellationToken);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(filePath)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(imageType);

        await minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        var presignedObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(filePath)
            .WithExpiry(MessageConstants.PresignedUrlExpirationInMinutes);

        var presignedUrl = await minioClient.PresignedGetObjectAsync(presignedObjectArgs);

        var fileName = filePath.Substring(filePath.LastIndexOf('/') + 1) ?? filePath;
        return (filePath, presignedUrl, fileName);
    }

    private async Task CreateOrCheckIfExistBucketAsync(CancellationToken cancellationToken)
    {
        var beArgs = new BucketExistsArgs().WithBucket(BucketName);
        bool found = await minioClient.BucketExistsAsync(beArgs);
        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(BucketName);
            await minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }
    }
}
