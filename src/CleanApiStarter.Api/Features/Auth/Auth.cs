namespace CleanApiStarter.Api.Features.Auth;

public sealed class Auth : IEndpointGroup
{
    public static int MajorVersion => 1;

    public static string RoutePrefix => "/api/auth";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        SignInWithGoogle.Map(groupBuilder);
        GetCurrentUser.Map(groupBuilder);
    }
}
