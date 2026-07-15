namespace Lumora.Application.Helpers;

public static class MessageConstants
{
    #region MinioConstants
    public const int PresignedUrlExpirationInMinutes = 60;
    public static class FolderPaths
    {
        public const string Studios = "studio";
        public const string Users = "users";
    }

    public static class ImageTypes
    {
        public const string Logo = "logo";
        public const string Cover = "cover";
        public const string Portfolio = "portfolio";
        public const string Avatar = "avatar";
    }

    public static readonly List<string> AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];
    
    public const long MaxFileSizeInBytes = 15 * 1024 * 1024;
    #endregion
}
