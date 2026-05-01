namespace CleanApiStarter.Application.Features.Auth;

public sealed class GoogleSignInDto
{
    public required string IdToken { get; init; }
}