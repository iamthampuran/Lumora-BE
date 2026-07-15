using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Lumora.Application.Configuration;

public class AppSettingsConfiguration
{
    [ValidateObjectMembers]
    public ConnectionStringsConfig ConnectionStrings { get; set; } = null!;
    [ValidateObjectMembers]
    public MinIOConfig Minio {  get; set; } = null!;
}

public class ConnectionStringsConfig
{
    [Required]
    public string DefaultConnection { get; set; } = null!;
}

public class MinIOConfig
{
    [Required]
    public string Endpoint { get; set; } = null!;
    [Required]
    public string AccessKey { get; set; } = null!;
    [Required]
    public string SecretKey { get; set; } = null!;
    [Required]
    public string BucketName { get; set; } = null!;
    [Required]
    public string UseSSL { get; set; } = null!;
}