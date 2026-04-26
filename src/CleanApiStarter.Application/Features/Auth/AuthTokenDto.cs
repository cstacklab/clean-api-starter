namespace CleanApiStarter.Application.Features.Auth;

public sealed class AuthTokenDto
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; init; }
}