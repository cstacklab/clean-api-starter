namespace CleanApiStarter.Api.Features.Auth;

public sealed class AuthTokenDto
{
    public required string AccessToken { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}
