namespace CleanApiStarter.Api.Features.Auth;

public static class GetCurrentUser
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", Handle)
            .RequireAuthorization()
            .WithName("GetCurrentUserV1");
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal principal,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        CurrentUserDto currentUser = await authService.GetCurrentUserAsync(principal, cancellationToken);

        return TypedResults.Ok(currentUser);
    }
}
