namespace CleanApiStarter.Configuration;

public sealed class JwtAuthenticationSettings
{
    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    [MinLength(32)]
    public string SigningKey { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpirationMinutes { get; set; } = 60;
}