namespace CleanApiStarter.Api.Features.Auth;

public static class SignInWithGoogle
{
    public sealed record Request(string IdToken);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(request => request.IdToken)
                .NotEmpty();
        }
    }

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/google", Handle)
            .AllowAnonymous()
            .WithName("SignInWithGoogleV1");
    }

    private static async Task<IResult> Handle(
        Request request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        AuthTokenDto token = await authService.SignInWithGoogleAsync(request.IdToken, cancellationToken);

        return TypedResults.Ok(token);
    }
}
