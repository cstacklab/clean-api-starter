namespace CleanApiStarter.Api.Features.Auth;

public interface IAuthService
{
    Task<AuthTokenDto> SignInWithGoogleAsync(GoogleSignInDto signInDto, CancellationToken cancellationToken);

    Task<CurrentUserDto> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}
