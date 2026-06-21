namespace CleanApiStarter.Api.Common.Interfaces;

public static class UserExtensions
{
    /// <summary>
    /// Returns the authenticated user's id, or throws if the request is unauthenticated.
    /// Endpoints are protected by authorization, so this is a defensive guard.
    /// </summary>
    public static string RequireId(this IUser user)
    {
        return user.Id ?? throw new UnauthorizedAccessException();
    }
}
