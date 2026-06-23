namespace CleanApiStarter.Api.Configuration;

public sealed class GoogleAuthenticationSettings
{
    [Required]
    public string ClientId { get; set; } = string.Empty;
}
