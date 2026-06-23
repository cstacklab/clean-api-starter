namespace CleanApiStarter.Api.Configuration;

public sealed class ConnectionStringSettings
{
    [Required]
    public string Postgres { get; set; } = string.Empty;
}
