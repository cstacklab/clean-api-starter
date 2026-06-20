namespace CleanApiStarter.Api.Configuration;

public sealed class AuthenticationSettings
{
    [Required]
    [ValidateObjectMembers]
    public GoogleAuthenticationSettings Google { get; set; } = new();

    [Required]
    [ValidateObjectMembers]
    public JwtAuthenticationSettings Jwt { get; set; } = new();
}
