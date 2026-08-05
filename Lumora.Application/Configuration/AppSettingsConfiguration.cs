using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Lumora.Application.Configuration;

public class AppSettingsConfiguration
{
    [ValidateObjectMembers]
    public ConnectionStringsConfig ConnectionStrings { get; set; } = null!;
    [ValidateObjectMembers]
    public MinIOConfig Minio {  get; set; } = null!;
    [ValidateObjectMembers]
    public SecurityConfig Security { get; set; } = null!;
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

public class SecurityConfig
{
    public Pbkdf2Config Pbkdf2 { get; set; } = null!;
    public JwtConfig Jwt { get; set; } = null!;
}

public class Pbkdf2Config
{
    [Required]
    public int IterationCount { get; set; }
    [Required]
    public int HashLength { get; set; }
}

public class JwtConfig
{
    [Required]
    public string SecretKey { get; set; } = null!;
    [Required]
    public int AccessTokenExpiryMinutes { get; set; } 
    [Required]
    public int RefreshTokenExpiryHours { get; set; }
}