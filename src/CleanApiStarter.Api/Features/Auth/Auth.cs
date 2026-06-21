namespace CleanApiStarter.Api.Features.Auth;

public sealed class Auth : IEndpointGroup
{
    public static int MajorVersion => 1;

    public static string RoutePrefix => "/api/auth";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/google", SignInWithGoogle)
            .AllowAnonymous()
            .WithName("SignInWithGoogleV1");

        groupBuilder.MapGet("/me", GetCurrentUser)
            .RequireAuthorization()
            .WithName("GetCurrentUserV1");
    }

    private static async Task<IResult> SignInWithGoogle(
        GoogleSignInDto signInDto,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        AuthTokenDto token = await authService.SignInWithGoogleAsync(signInDto, cancellationToken);

        return Results.Ok(token);
    }

    private static async Task<IResult> GetCurrentUser(
        ClaimsPrincipal principal,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        CurrentUserDto currentUser = await authService.GetCurrentUserAsync(principal, cancellationToken);

        return Results.Ok(currentUser);
    }
}
