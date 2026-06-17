namespace CleanApiStarter.AspNetCore;

public sealed class UserIdHttpLoggingInterceptor : IHttpLoggingInterceptor
{
    private const string UserIdParameterName = "UserId";
    private const string AnonymousUserId = "anonymous";

    public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
    {
        logContext.AddParameter(UserIdParameterName, GetUserId(logContext.HttpContext));

        return ValueTask.CompletedTask;
    }

    public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
    {
        logContext.AddParameter(UserIdParameterName, GetUserId(logContext.HttpContext));

        return ValueTask.CompletedTask;
    }

    private static string GetUserId(HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? AnonymousUserId;
    }
}
