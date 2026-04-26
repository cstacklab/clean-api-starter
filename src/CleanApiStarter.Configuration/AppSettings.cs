namespace CleanApiStarter.Configuration;

public sealed class AppSettings
{
    [Required]
    [ValidateObjectMembers]
    public ConnectionStringSettings ConnectionStrings { get; set; } = new();
}
