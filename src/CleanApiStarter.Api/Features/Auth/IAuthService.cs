namespace CleanApiStarter.Api.Features.Auth;

public interface IAuthService
{
    Task<AuthTokenDto> SignInWithGoogleAsync(string idToken, CancellationToken cancellationToken);

    Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}
