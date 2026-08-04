namespace Lumora.Application.Helpers;

public static class FileServiceHelper
{
    public static string GetStudioLogoUrl(string bucketName, string studioId)
    {
        return $"{bucketName}/studio/{studioId}/logo/";
    }

    public static string GetFileNameWithExtension(string fileName, string contentType = "image/jpeg")
    {
        var extension = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => throw new ArgumentException($"Unsupported content-type: {contentType}")
        };

        return $"{Path.GetFileNameWithoutExtension(fileName)}{extension}";
    }
}
